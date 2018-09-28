using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EventManager.Models
{
    public class Family
    {
        public Guid ID { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime UpdatedOn { get; set; }
        public List<FamilyMember> FamilyMembers { get; set; }
        public string FamilyName { get; set; }
    }
}
