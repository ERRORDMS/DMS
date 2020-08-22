using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DMS.Models
{
    public class Category
    {
        public long AutoKey { get; set; }
        public string Name { get; set; }
        public long FatherAutoKey { get; set; }
    }

    public class UserCategory
    {
        public long AutoKey { get; set; }
        public string Name { get; set; }
        public long FatherAutoKey { get; set; }
        public bool CanEdit { get; set; }
        public bool CanView { get; set; }
        public bool CanDelete { get; set; }
        public bool CanAdd { get; set; }
    }
}
