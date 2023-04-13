using System;

namespace DMS.Models
{
    public class Notification
    {
        public long ID { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedAt { get; set; }
        public bool MarkedAsRead { get; set; }
    }
}
