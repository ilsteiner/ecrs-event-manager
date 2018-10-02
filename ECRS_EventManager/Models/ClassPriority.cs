using EventManager.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace EventManager.Models
{
    public class ClassPriority
    { 

        [Required]
        [Key]
        public EventClass EventClass { get; set; }

        [Required]
        [Key]
        [Range(1,100)]
        public int Priority { get; set; }
    }
}
