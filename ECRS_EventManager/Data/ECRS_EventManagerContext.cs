using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using EventManager.Models;

namespace ECRS_EventManager.Models
{
    public class ECRS_EventManagerContext : DbContext
    {
        public ECRS_EventManagerContext (DbContextOptions<ECRS_EventManagerContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Event>()
                .HasAlternateKey("FormID");
        }

        public DbSet<EventManager.Models.Registration> Registration { get; set; }

        public DbSet<EventManager.Models.RegistrationEntry> RegistrationEntry { get; set; }

        public DbSet<EventManager.Models.Event> Event { get; set; }

        public DbSet<EventManager.Models.Person> Person { get; set; }

        public DbSet<EventManager.Models.Address> Address { get; set; }
    }
}
