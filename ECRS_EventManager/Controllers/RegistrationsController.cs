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
                registration.ID = Guid.NewGuid();

                // Get the related event model or create a new one if needed
                registration.Event = DeserializeEvent(json);

                // Get the related person (the payor) or create a new one if needed
                registration.Payor = await _context.Person.Where(m =>
                                (json["Email"] != null && m.Email == json["Email"].Value<string>()) ||
                                    ((json["MainContactName"]["First"] != null && json["MainContactName"]["Last"] != null) &&
                                    (m.FirstName == json["MainContactName"]["First"].Value<string>() &&
                                    m.LastName == json["MainContactName"]["Last"].Value<string>()))
                            )
                            .DefaultIfEmpty(await DeserializePayor(json))
                            .SingleOrDefaultAsync();

                // Default to empty string or current time on missing property
                registration.FormEntryID = json["Entry"]["Number"] == null ? String.Empty
                                            : json["Entry"]["Number"].Value<string>();
                registration.FormAdminLink = json["Entry"]["AdminLink"] == null ? String.Empty
                                            : json["Entry"]["AdminLink"].Value<string>();
                registration.FormEditLink = json["Entry"]["EditLink"] == null ? String.Empty
                                            : json["Entry"]["EditLink"].Value<string>();
                registration.CreatedOn = json["Entry"]["DateCreated"] == null ? DateTime.Now
                                            : json["Entry"]["DateCreated"].Value<DateTime>();
                registration.SubmittedOn = json["Entry"]["DateSubmitted"] == null ? DateTime.Now 
                                            : json["Entry"]["DateSubmitted"].Value<DateTime>();
                registration.UpdatedOn = json["Entry"]["DateUpdated"] == null ? DateTime.Now
                                            : json["Entry"]["DateUpdated"].Value<DateTime>();

                // Lookup each person or create them if needed
                if(json["Attendees"] != null && json["Attendees"].Count() > 0)
                {
                    foreach (var attendee in json["Attendees"])
                    {
                        // Look up person in DB or get them from the JSON
                        Person person = await DeserializeAttendee(attendee);

                        RegistrationEntry entry = new RegistrationEntry()
                        {
                            ID = Guid.NewGuid(),
                            CreatedOn = json["Entry"]["DateCreated"].Value<DateTime>(),
                            SubmittedOn = json["Entry"]["DateSubmitted"].Value<DateTime>(),
                            UpdatedOn = json["Entry"]["DateUpdated"].Value<DateTime>(),
                            Person = person
                        };

                        if (registration.Entries == null)
                        {
                            registration.Entries = new List<RegistrationEntry>() { entry };
                        }
                        else
                        {
                            registration.Entries.Add(entry);
                        }
                    }
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

        private Event DeserializeEvent(in JObject json)
        {
            string FormID = json["Form"]["Id"].Value<string>();
            Event theEvent = _context.Event.Where(m => m.FormID == FormID).SingleOrDefault();

            if (theEvent != null)
            {
                _context.Attach(theEvent);

                return theEvent;
            }
            else
            {
                theEvent = new Event()
                {
                    ID = Guid.NewGuid(),
                    CreatedOn = DateTime.Now,
                    UpdatedOn = DateTime.Now,
                    EventName = json["Form"]["Name"].Value<string>(),
                    FormID = json["Form"]["Id"].Value<string>(),
                    FormName = json["Form"]["Name"].Value<string>(),
                    FormInternalName = json["Form"]["InternalName"].Value<string>()
                };

                _context.Add(theEvent);

                return theEvent;
            }
        }

        async private Task<Person> DeserializePayor(JObject json)
        {
            bool isNew = false;

            Address billingAddress = await DeserializeBillingAddress(json);

            Person thePerson = await _context.Person.Where(m =>
                                (json["Email"] != null && m.Email == json["Email"].Value<string>()) ||
                                    ((json["MainContactName"]["First"] != null && json["MainContactName"]["Last"] != null) &&
                                    (m.FirstName == json["MainContactName"]["First"].Value<string>() &&
                                    m.LastName == json["MainContactName"]["Last"].Value<string>()))
                            )
                            .SingleOrDefaultAsync();

            // Create a new person
            if(thePerson == null)
            {
                isNew = true;

                thePerson = new Person()
                {
                    ID = Guid.NewGuid(),
                    CreatedOn = DateTime.Now
                };
            }

            // The following code works regardless of if it's a new person or an updated existing one

            thePerson.UpdatedOn = DateTime.Now;

            // If there's a new email that isn't an empty string use that, otherwise use existing email
            // Existing email might be null or empty, but whatever
            thePerson.Email = json["Email"] == null || json["Email"].Value<string>() == String.Empty
                ? thePerson.Email : json["Email"].Value<string>();

            // Only add the address if the person doesn't have any and/or doesn't have this one
            if(thePerson.Addresses == null)
            {
                thePerson.Addresses = new List<Address>() { billingAddress };
            }
            else if (!thePerson.Addresses.Contains(billingAddress))
            {
                thePerson.Addresses.Add(billingAddress);
            }

            //Populate the phone number, use the first attendee if needed
            if(json["Phone"] != null && json["Phone"].Value<string>() != String.Empty)
            {
                thePerson.Phone = json["Phone"].Value<string>();
            }
            else if(json["Attendees"][0]["Phone"] != null && json["Attendees"][0]["Phone"].Value<string>() != String.Empty)
            {
                thePerson.Phone = json["Attendees"][0]["Phone"].Value<string>();
            }

            // Populate the name
            // Use the billing name if present, otherwise use the first attendee
            PopulateName(ref thePerson, 
                json["MainContactName"], 
                new List<JToken>() { json["Attendees"][0]["Name"] });

            if (isNew)
            {
                _context.Add(thePerson);
            }
            else
            {
                _context.Update(thePerson);
            }

            return thePerson;
        }

        async private Task<Address> DeserializeBillingAddress(JObject json)
        {
            Address billingAddress;

            // Default missing fields to empty strings
            if (_context.Address.Count() != 0)
            {
                billingAddress = await _context.Address.Where(m =>
                        json["Address"]["Line1"] != null &&
                        json["Address"]["City"] != null &&
                        json["Address"]["State"] != null &&
                        m.Line1 == json["Address"]["Line1"].Value<string>() &&
                        m.City == json["Address"]["City"].Value<string>() &&
                        m.State == json["Address"]["State"].Value<string>())
                        .DefaultIfEmpty(new Address()
                        {
                            ID = Guid.NewGuid(),
                            CreatedOn = DateTime.Now,
                            UpdatedOn = DateTime.Now,
                            City = json["Address"]["City"] == null ? String.Empty 
                                    : json["Address"]["City"].Value<string>(),
                            Line1 = json["Address"]["Line1"] == null ? String.Empty
                                    : json["Address"]["Line1"].Value<string>(),
                            Line2 = json["Address"]["Line2"] == null ? String.Empty
                                    : json["Address"]["Line2"].Value<string>(),
                            Line3 = json["Address"]["Line3"] == null ? String.Empty
                                    : json["Address"]["Line3"].Value<string>(),
                            Country = json["Address"]["Country"] == null ? String.Empty 
                                    : json["Address"]["Country"].Value<string>(),
                            State = json["Address"]["State"] == null ? String.Empty
                                    : json["Address"]["State"].Value<string>(),
                            PostalCode = json["Address"]["PostalCode"] == null ? String.Empty
                                    : json["Address"]["PostalCode"].Value<string>(),
                            Type = json["Address"]["Type"] == null ? String.Empty
                                    : json["Address"]["Type"].Value<string>(),
                            IsPrimary = true
                        })
                        .SingleAsync();
            }
            else
            {
                billingAddress = new Address()
                {
                    ID = Guid.NewGuid(),
                    CreatedOn = DateTime.Now,
                    UpdatedOn = DateTime.Now,
                    City = json["Address"]["City"] == null ? String.Empty
                                    : json["Address"]["City"].Value<string>(),
                    Line1 = json["Address"]["Line1"] == null ? String.Empty
                                    : json["Address"]["Line1"].Value<string>(),
                    Line2 = json["Address"]["Line2"] == null ? String.Empty
                                    : json["Address"]["Line2"].Value<string>(),
                    Line3 = json["Address"]["Line3"] == null ? String.Empty
                                    : json["Address"]["Line3"].Value<string>(),
                    Country = json["Address"]["Country"] == null ? String.Empty
                                    : json["Address"]["Country"].Value<string>(),
                    State = json["Address"]["State"] == null ? String.Empty
                                    : json["Address"]["State"].Value<string>(),
                    PostalCode = json["Address"]["PostalCode"] == null ? String.Empty
                                    : json["Address"]["PostalCode"].Value<string>(),
                    Type = json["Address"]["Type"] == null ? String.Empty
                                    : json["Address"]["Type"].Value<string>(),
                    IsPrimary = true
                };
            }

            return billingAddress;
        }

        async private Task<Person> DeserializeAttendee(JToken attendee, Address address = null)
        {
            Person thePerson = await _context.Person.Where(m =>
                                (attendee["Email"] != null && m.Email == attendee["Email"].Value<string>()) ||
                                    ((attendee["Name"]["First"] != null && attendee["Name"]["Last"] != null) &&
                                    (m.FirstName == attendee["Name"]["First"].Value<string>() &&
                                    m.LastName == attendee["Name"]["Last"].Value<string>()))
                            )
                            .SingleOrDefaultAsync();

            if(thePerson == null)
            {
                thePerson = new Person()
                {
                    ID = Guid.NewGuid(),
                    CreatedOn = DateTime.Now,
                    Addresses = new List<Address>()
                };
            }

            thePerson.UpdatedOn = DateTime.Now;
            thePerson.Phone = attendee["Phone"] == null && attendee["Phone"].Value<string>() != String.Empty 
                                ? thePerson.Phone : attendee["Phone"].Value<string>();
            thePerson.Gender = attendee["Gender"] == null && attendee["Gender"].Value<string>() != String.Empty
                                ? thePerson.Gender : attendee["Gender"].Value<string>();
            thePerson.Email = attendee["Email"] == null && attendee["Email"].Value<string>() != String.Empty
                                ? thePerson.Email : attendee["Email"].Value<string>();

            if(address != null)
            {
                // If this person doesn't have this address yet
                if(!thePerson.Addresses.Contains(address))
                {
                    thePerson.Addresses.Add(address);
                }                
            }

            PopulateName(ref thePerson, attendee["Name"]);

            return thePerson;
        }

        private void PopulateName(ref Person person, JToken name, List<JToken> secondaryName = null)
        {
            // Use existing value if looked up value is null
            // We never overwrite an existing value with a null or empty value
            person.FirstName = name["First"] == null || name["First"].Value<string>() == String.Empty
                                ? person.FirstName
                                : name["First"].Value<string>();
            person.LastName = name["Last"] == null || name["Last"].Value<string>() == String.Empty
                                ? person.LastName
                                : name["Last"].Value<string>();
            person.MiddleName = name["Middle"] == null || name["Middle"].Value<string>() == String.Empty
                                ? person.MiddleName
                                : name["Middle"].Value<string>();
            person.MiddleInitial = name["MiddleInitial"] == null || name["MiddleInitial"].Value<string>() == String.Empty
                                ? person.MiddleInitial
                                : name["MiddleInitial"].Value<string>();
            person.NamePrefix = name["NamePrefix"] == null || name["NamePrefix"].Value<string>() == String.Empty
                                ? person.NamePrefix
                                : name["NamePrefix"].Value<string>();
            person.NameSuffix = name["NameSuffix"] == null || name["NameSuffix"].Value<string>() == String.Empty
                                ? person.NameSuffix
                                : name["NameSuffix"].Value<string>();

            // If we have a list of alternate names, try those too
            if (secondaryName != null && secondaryName.Count > 0)
            {
                // Call this method again for the next name candidate on the list
                JToken nextName = secondaryName.First();
                secondaryName.RemoveAt(0);
                PopulateName(ref person, nextName, secondaryName.Count() > 0 ? secondaryName : null);
            }
        }
    }
}