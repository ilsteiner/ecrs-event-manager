using EventManager.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace EventManager.Models
{
    public class EventClass
    {
        [Required]
        public Guid ID { get; set; }

        [Required]
        public DateTime CreatedOn { get; set; }

        public DateTime UpdatedOn { get; set; }

        [Required]
        public Event Event { get; set; }

        [Required]
        public string EventPeriod { get; set; }

        [Required]
        public string Name { get; set; }

        public string Description { get; set; }

        public string Leader { get; set; }
    }
}
