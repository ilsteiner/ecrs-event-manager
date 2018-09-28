using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace EventManager.Models
{
    public class Registration
    {
        [Required]
        public Guid ID { get; set; }

        [Required]
        public Event Event { get; set; }

        [Required]
        public string FormEntryID { get; set; }

        [Required]
        public string FormAdminLink { get; set; }

        [Required]
        public string FormEditLink { get; set; }

        [Required]
        public DateTime CreatedOn { get; set; }

        [Required]
        public DateTime SubmittedOn { get; set; }

        public DateTime UpdatedOn { get; set; }

        [Required]
        public Person Payor { get; set; }

        [Required]
        public List<RegistrationEntry> Entries { get; set; }
    }
}
