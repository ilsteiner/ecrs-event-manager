using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ECRS_EventManager.Models;
using EventManager.Models;
using Newtonsoft.Json.Linq;

namespace ECRS_EventManager.Controllers
{
    [Produces("application/json")]
    [Route("api/Registrations")]
    public class RegistrationsController : Controller
    {
        private readonly ECRS_EventManagerContext _context;

        public RegistrationsController(ECRS_EventManagerContext context)
        {
            _context = context;
        }

        // GET: api/Registrations
        [HttpGet]
        public IEnumerable<Registration> GetRegistration()
        {
            return _context.Registration;
        }

        // GET: api/Registrations/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetRegistration([FromRoute] Guid id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var registration = await _context.Registration.SingleOrDefaultAsync(m => m.ID == id);

            if (registration == null)
            {
                return NotFound();
            }

            return Ok(registration);
        }

        // PUT: api/Registrations/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutRegistration([FromRoute] Guid id, [FromBody] Registration registration)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != registration.ID)
            {
                return BadRequest();
            }

            _context.Entry(registration).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!RegistrationExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Registrations
        //[HttpPost]
        //public async Task<IActionResult> PostRegistration([FromBody] Registration registration)
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        return BadRequest(ModelState);
        //    }

        //    _context.Registration.Add(registration);
        //    await _context.SaveChangesAsync();

        //    return CreatedAtAction("GetRegistration", new { id = registration.ID }, registration);
        //}

        // POST: api/Registrations
        [HttpPost]
        public async Task<IActionResult> PostRegistration([FromBody] JObject json)
        {
            Registration registration = new Registration();

            try
            {
                registration.ID = new Guid();

                // Get the related event model or create a new one if needed
                var relatedEvent = await _context.Event.Where(m => m.FormID == json["Form"]["Id"].Value<string>())
                                        .DefaultIfEmpty(DeserializeEvent(json))
                                        .SingleAsync();

                // Get the related person (the payor) or create a new one if needed
                var relatedPerson = await _context.Person.Where(m =>
                                m.Email == json["Entry"]["Order"]["EmailAddress"].Value<string>() ||
                                    (m.FirstName == json["Entry"]["Order"]["BillingName"]["First"].Value<string>() &&
                                    m.LastName == json["Entry"]["Order"]["BillingName"]["Last"].Value<string>())
                            ).SingleAsync();

                registration.FormEntryID = json["Entry"]["Number"].FirstOrDefault().Value<string>();
                registration.FormAdminLink = json["Entry"]["AdminLink"].FirstOrDefault().Value<string>();
                registration.FormEditLink = json["Entry"]["EditLink"].FirstOrDefault().Value<string>();
                registration.CreatedOn = json["Entry"]["DateCreated"].FirstOrDefault().Value<DateTime>();
                registration.SubmittedOn = json["Entry"]["DateSubmitted"].FirstOrDefault().Value<DateTime>();
                registration.UpdatedOn = json["Entry"]["DateUpdated"].FirstOrDefault().Value<DateTime>();
                registration.Payor = await DeserializePayor(json);

                foreach (var attendee in json["Attendees"])
                {
                    // Lookup each person or create them if needed
                }

                _context.Registration.Add(registration);

                await _context.SaveChangesAsync();

                return CreatedAtAction("GetRegistration", new { id = registration.ID }, registration);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        // DELETE: api/Registrations/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteRegistration([FromRoute] Guid id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var registration = await _context.Registration.SingleOrDefaultAsync(m => m.ID == id);
            if (registration == null)
            {
                return NotFound();
            }

            _context.Registration.Remove(registration);
            await _context.SaveChangesAsync();

            return Ok(registration);
        }

        private bool RegistrationExists(Guid id)
        {
            return _context.Registration.Any(e => e.ID == id);
        }

        public Event DeserializeEvent(in JObject json)
        {
            Event newEvent = new Event()
            {
                ID = new Guid(),
                CreatedOn = DateTime.Now,
                UpdatedOn = DateTime.Now,
                EventName = json["Form"]["Name"].Value<string>(),
                FormID = json["Form"]["Id"].Value<string>(),
                FormName = json["Form"]["Name"].Value<string>(),
                FormInternalName = json["Form"]["InternalName"].Value<string>()
            };

            _context.Add(newEvent);

            return newEvent;
        }

        async public Task<Person> DeserializePayor(JObject json)
        {
            Address billingAddress = await _context.Address.Where(m =>
                        m.Line1 == json["Entry"]["Order"]["BillingAddress"]["Line1"].Value<string>() &&
                        m.City == json["Entry"]["Order"]["BillingAddress"]["City"].Value<string>() &&
                        m.State == json["Entry"]["Order"]["BillingAddress"]["State"].Value<string>())
                        .DefaultIfEmpty(new Address()
                        {
                            ID = new Guid(),
                            CreatedOn = DateTime.Now,
                            UpdatedOn = DateTime.Now,
                            City = json["Entry"]["Order"]["BillingAddress"]["City"].Value<string>(),
                            Line1 = json["Entry"]["Order"]["BillingAddress"]["Line1"].Value<string>(),
                            Line2 = json["Entry"]["Order"]["BillingAddress"]["Line2"].Value<string>(),
                            Line3 = json["Entry"]["Order"]["BillingAddress"]["Line3"].Value<string>(),
                            Country = json["Entry"]["Order"]["BillingAddress"]["Country"].Value<string>(),
                            State = json["Entry"]["Order"]["BillingAddress"]["State"].Value<string>(),
                            PostalCode = json["Entry"]["Order"]["BillingAddress"]["PostalCode"].Value<string>(),
                            Type = json["Entry"]["Order"]["BillingAddress"]["Type"].Value<string>(),
                            IsPrimary = true
                        })
                        .SingleAsync();                        
                        
            Person newPerson = new Person()
            {
                ID = new Guid(),
                CreatedOn = DateTime.Now,
                UpdatedOn = DateTime.Now,
                // Use the billing name if present, otherwise use the first attendee
                FirstName = json["Entry"]["Order"]["BillingName"]["First"]
                    .DefaultIfEmpty(json["Attendees"][0]["Name"]["First"])
                    .Value<string>(),
                LastName = json["Entry"]["Order"]["BillingName"]["Last"]
                    .DefaultIfEmpty(json["Attendees"][0]["Name"]["Last"])
                    .Value<string>(),
                MiddleName = json["Entry"]["Order"]["BillingName"]["Middle"]
                    .DefaultIfEmpty(json["Attendees"][0]["Name"]["Middle"])
                    .Value<string>(),
                MiddleInitial = json["Entry"]["Order"]["BillingName"]["MiddleInitial"]
                    .DefaultIfEmpty(json["Attendees"][0]["Name"]["MiddleInitial"])
                    .Value<string>(),
                NamePrefix = json["Entry"]["Order"]["BillingName"]["Prefix"]
                    .DefaultIfEmpty(json["Attendees"][0]["Name"]["Prefix"])
                    .Value<string>(),
                NameSuffix = json["Entry"]["Order"]["BillingName"]["Suffix"]
                    .DefaultIfEmpty(json["Attendees"][0]["Name"]["Suffix"])
                    .Value<string>(),
                Phone = json["Entry"]["Order"]["PhoneNumber"]
                    .DefaultIfEmpty(json["Attendees"][0]["Phone"])
                    .Value<string>(),
                Email = json["Entry"]["Order"]["EmailAddress"]
                    .Value<string>(),

                // Get the related address or create a new one if needed
                BillingAddress = billingAddress,
                Addresses = new List<Address>() { billingAddress }
            };

            _context.Add(newPerson);
            return newPerson;
        }
    }
}