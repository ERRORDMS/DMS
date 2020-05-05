using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DMS.Models
{
    public class Document
    {
        public long InfoAutoKey { get; set; }
        public DateTime DateTimeAdded { get; set; }
        public string Name { get; set; }
        public string Ext { get; set; }
    }
}
