﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using ECRS_EventManager.Models;
using EventManager.Models;

namespace ECRS_EventManager.Pages.People
{
    public class IndexModel : PageModel
    {
        private readonly ECRS_EventManager.Models.ECRS_EventManagerContext _context;

        public IndexModel(ECRS_EventManager.Models.ECRS_EventManagerContext context)
        {
            _context = context;
        }

        public IList<Person> Person { get;set; }

        public async Task OnGetAsync()
        {
            Person = await _context.Person.ToListAsync();
        }
    }
}
