using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DMS.Models
{
    public class EmailItem
    {
        public long AutoKey { get; set; }
        public string From { get; set; }
        public string To { get; set; }
        public string Content { get; set; }
        public string Subject { get; set; }
    }
}
