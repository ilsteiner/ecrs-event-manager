using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace EventManager.Models
{
    public class RegistrationEntry
    {
        [Required]
        public Guid ID { get; set; }

        [Required]
        public DateTime CreatedOn { get; set; }

        [Required]
        public DateTime SubmittedOn { get; set; }

        public DateTime UpdatedOn { get; set; }

        [Required]
        public Person Person { get; set; }
    }
}
