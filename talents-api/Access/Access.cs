using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TalentsApi.Access
{
    public class UserAccess
    {
        public const string View = "Pages.Administration.Users";
        public const string Create = "Pages.Administration.Users.Create";
        public const string Update = "Pages.Administration.Users.Update";
        public const string Delete = "Pages.Administration.Users.Delete";
        public const string Lock = "Pages.Administration.Users.Lock";
        public const string Impersonate = "Pages.Administration.Users.Impersonate";
        public const string SpecialPermission = "Pages.Administration.Users.SpecialPermission";
    }

    public class UserRoleAccess
    {
        public const string View = "Pages.Administration.Roles";
        public const string Create = "Pages.Administration.Roles.Create";
        public const string Update = "Pages.Administration.Roles.Update";
        public const string Delete = "Pages.Administration.Roles.Delete";
    }
}
