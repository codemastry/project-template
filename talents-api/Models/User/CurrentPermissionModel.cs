using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TalentsApi.Models.UserRole;

namespace TalentsApi.Models.User
{
    public class CurrentPermissionModel
    {
        public List<string> GrantedPermissions { get; set; } = new List<string>();
        public List<UserRoleTreeItemModel> AllPermissions { get; set; } = new List<UserRoleTreeItemModel>();
    }
}
