using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DMS.Models
{
    public class Contact
    {
        public long AutoKey { get; set; }
        public string Name { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public string CategoryID { get; set; }
    }

    public class ContactCategory
    {
        public long AutoKey { get; set; }
        public string Name { get; set; }
    }
}
