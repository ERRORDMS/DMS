using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DMS.Models
{
    public class DeletedFile
    {
        public string Email { get; set; }
        public string FileName { get; set; }
        public DateTime DeletionDate { get; set; }
        public long DocumentAutoKey { get; set; }
        public long LineAutoKey { get; set; }
    }
}
