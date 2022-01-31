using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TalentsApi.Access
{
    public class PermissionList
    {
        public static List<Permission> Permissions
        {
            get
            {
                var permissions = new List<Permission>();
                permissions.Add(new Permission
                {
                    Id = "Pages",
                    Name = "Pages",
                    ParentId = null,
                });
                permissions.Add(new Permission
                {
                    Id = "Pages.Dashboard",
                    Name = "Dashboard",
                    ParentId = "Pages",
                });
                permissions.Add(new Permission
                {
                    Id = "Pages.Administration",
                    Name = "Administration",
                    ParentId = "Pages",
                });

                #region Roles
                permissions.Add(new Permission
                {
                    Id = "Pages.Administration.Roles",
                    Name = "Roles",
                    ParentId = "Pages.Administration",
                });
                permissions.Add(new Permission
                {
                    Id = "Pages.Administration.Roles.Create",
                    Name = "Creating new role",
                    ParentId = "Pages.Administration.Roles",
                });
                permissions.Add(new Permission
                {
                    Id = "Pages.Administration.Roles.Update",
                    Name = "Editing role",
                    ParentId = "Pages.Administration.Roles",
                });
                permissions.Add(new Permission
                {
                    Id = "Pages.Administration.Roles.Delete",
                    Name = "Deleting role",
                    ParentId = "Pages.Administration.Roles",
                });
                #endregion

                #region Users
                permissions.Add(new Permission
                {
                    Id = "Pages.Administration.Users",
                    Name = "Users",
                    ParentId = "Pages.Administration",
                });
                permissions.Add(new Permission
                {
                    Id = "Pages.Administration.Users.Create",
                    Name = "Creating new user",
                    ParentId = "Pages.Administration.Users",
                });
                permissions.Add(new Permission
                {
                    Id = "Pages.Administration.Users.Update",
                    Name = "Editing user",
                    ParentId = "Pages.Administration.Users",
                });
                permissions.Add(new Permission
                {
                    Id = "Pages.Administration.Users.Delete",
                    Name = "Deleting user",
                    ParentId = "Pages.Administration.Users",
                });
                permissions.Add(new Permission
                {
                    Id = "Pages.Administration.Users.Lock",
                    Name = "Locking/Unlocking user",
                    ParentId = "Pages.Administration.Users",
                });
                permissions.Add(new Permission
                {
                    Id = "Pages.Administration.Users.Impersonate",
                    Name = "Login as user",
                    ParentId = "Pages.Administration.Users",
                });
                permissions.Add(new Permission
                {
                    Id = "Pages.Administration.Users.SpecialPermission",
                    Name = "Manage user's special permissions",
                    ParentId = "Pages.Administration.Users",
                });
                #endregion
                return permissions;
            }
        }
    }
}
