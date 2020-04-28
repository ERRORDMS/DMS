using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace DMS.Areas.Identity.Data
{
    // Add profile data for application users by adding properties to the DMSUser class
    public class DMSUser : IdentityUser
    {
        public string email { get; set; }
        public string username { get; set; }
        public string password { get; set; }
    }
}
