using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using ECRS_EventManager.Models;
using EventManager.Models;

namespace ECRS_EventManager.Pages.Events
{
    public class DetailsModel : PageModel
    {
        private readonly ECRS_EventManager.Models.ECRS_EventManagerContext _context;

        public DetailsModel(ECRS_EventManager.Models.ECRS_EventManagerContext context)
        {
            _context = context;
        }

        public Event Event { get; set; }

        public async Task<IActionResult> OnGetAsync(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Event = await _context.Event.SingleOrDefaultAsync(m => m.ID == id);

            if (Event == null)
            {
                return NotFound();
            }
            return Page();
        }
    }
}
