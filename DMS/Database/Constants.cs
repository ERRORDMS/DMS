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
        public const string SearchKeys = "SearchKeys";
        public const string DocumentInfo = "DocumentsInfo";
        public const string DocumentLines = "DocumentLine";
        public const string DocumentCategoryRel = "DocumentCatRel";
        public const string DocumentContactRel = "DocumentContactsRel";
        public const string DocumentSearchKeysRel = "DocumentSearchKeysRel";
        public const string CategoryUserRel = "CategoryUserRel";
        public const string Images = "Images";
    }
}
