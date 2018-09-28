using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace EventManager.Models
{
    public class Person
    {
        [Required]
        public Guid ID { get; set; }

        [Required]
        public DateTime CreatedOn { get; set; }

        public DateTime UpdatedOn { get; set; }

        [Required]
        [StringLength(255)]
        public string FirstName { get; set; }

        [Required]
        [StringLength(255)]
        public string LastName { get; set; }

        [StringLength(255)]
        public string MiddleName { get; set; }

        [StringLength(2)]
        public string MiddleInitial { get; set; }

        [StringLength(31)]
        public string NamePrefix { get; set; }

        [StringLength(31)]
        public string NameSuffix { get; set; }

        public string Phone { get; set; }
        public string Gender { get; set; }
        public string Email { get; set; }

        [Required]
        public Address BillingAddress { get; set; }

        [Required]
        public List<Address> Addresses { get; set; }
    }
}
