using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EventManager.Models
{
    public class FamilyMember
    {
        public Guid ID { get; set; }
        public Person Person { get; set; }
        public string Role { get; set; }
        public string Notes { get; set; }
    }
}
