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
        public long FatherID { get; set; }
    }
}
