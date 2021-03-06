﻿using DevExpress.Utils.Text;
using Microsoft.EntityFrameworkCore.ValueGeneration.Internal;
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
        public const string TrustedIPs = "TrustedIPs";
        public const string Roles = "Roles";
        public const string EnterpriseCodes = "EnterpriseCodes";
        public const string Permissions = "Permissions";
        public const string UserPermissions = "UserPermissions";
        public const string RoleCategories = "RoleCategories";
        public const string UserRoles = "UserRoles";
        public const string RolePermissions = "RolePermissions";
        public const string Categories = "DocumentCatTree";
        public const string UserCategories = "UserCategories";
        public const string Contacts = "Contacts";
        public const string SearchKeys = "SearchKeys";
        public const string DocumentInfo = "DocumentsInfo";
        public const string DocumentLines = "DocumentLine";
        public const string DocumentCategoryRel = "DocumentCatRel";
        public const string DocumentContactRel = "DocumentContactsRel";
        public const string DocumentSearchKeysRel = "DocumentSearchKeysRel";
        public const string CategoryUserRel = "CategoryUserRel";
        public const string Images = "Images";
        public const string UserStorage = "UserStorage";
        public const string UserDatabases = "UserDB";
    }
}
