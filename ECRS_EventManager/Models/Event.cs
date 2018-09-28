using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EventManager.Models
{
    public class Event
    {
        public Guid ID { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime UpdatedOn { get; set; }
        public string EventName { get; set; }
        public string FormID { get; set; }
        public string FormInternalName { get; set; }
        public string FormName { get; set; }
    }
}
