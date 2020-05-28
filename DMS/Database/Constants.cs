using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DMS.Database
{
    public class Constants
    {

    }

    public class Tables
    {
        public const string Users = "users";
        public const string Categories = "DocumentCatTree";
        public const string Contacts = "Contacts";
        public const string DocumentInfo = "DocumentsInfo";
        public const string DocumentLines = "DocumentLines";
        public const string DocumentCategoryRel = "DocumentCategoryRel";
        public const string DocumentContactRel = "DocumentContactRel";
        public const string CategoryUserRel = "CategoryUserRel";
    }
}
