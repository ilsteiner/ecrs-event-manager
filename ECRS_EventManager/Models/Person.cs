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
        [StringLength(255, ErrorMessage = "{0} can have a max of {1} characters")]
        public string FirstName { get; set; }

        [Required]
        [StringLength(255, ErrorMessage = "{0} can have a max of {1} characters")]
        public string LastName { get; set; }

        [StringLength(255, ErrorMessage = "{0} can have a max of {1} characters")]
        public string MiddleName { get; set; }

        [StringLength(2, ErrorMessage = "{0} can have a max of {1} characters")]
        public string MiddleInitial { get; set; }

        [StringLength(31, ErrorMessage = "{0} can have a max of {1} characters")]
        public string NamePrefix { get; set; }

        [StringLength(31, ErrorMessage = "{0} can have a max of {1} characters")]
        public string NameSuffix { get; set; }

        [Phone]
        public string Phone { get; set; }

        public string Gender { get; set; }

        [EmailAddress]
        public string Email { get; set; }

        [Required]
        public List<Address> Addresses { get; set; }
    }
}
