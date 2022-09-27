using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DMS.Models
{
    public class Document
    {
        public long ID { get; set; }       
        public string UserID { get; set; }
        public long DocumentAutoKey { get; set; }
        public long LineAutoKey { get; set; }
        public DateTime DateTimeAdded { get; set; }
        public string Name { get; set; }
        public string Ext { get; set; }
        public string Note { get; set; }
        public long Size { get; set; }
    }

    public class FileTypeSize
    {
        public string Name { get; set; }
        public long Files { get; set; }
        public long Size { get; set; }
    }
}
