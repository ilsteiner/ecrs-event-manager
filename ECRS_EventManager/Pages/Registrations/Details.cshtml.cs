using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using ECRS_EventManager.Models;
using EventManager.Models;

namespace ECRS_EventManager.Pages.Registrations
{
    public class DetailsModel : PageModel
    {
        private readonly ECRS_EventManager.Models.ECRS_EventManagerContext _context;

        public DetailsModel(ECRS_EventManager.Models.ECRS_EventManagerContext context)
        {
            _context = context;
        }

        public Registration Registration { get; set; }

        public async Task<IActionResult> OnGetAsync(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Registration = await _context.Registration.SingleOrDefaultAsync(m => m.ID == id);

            if (Registration == null)
            {
                return NotFound();
            }
            return Page();
        }
    }
}
