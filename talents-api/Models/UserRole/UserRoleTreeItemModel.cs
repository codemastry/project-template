using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TalentsApi.Models.UserRole
{
    public class UserRoleTreeItemModel
    {
        public string Key { get; set; }
        public string Title { get; set; }
        public string ParentKey { get; set; }
        public List<UserRoleTreeItemModel> Children { get; set; } = new List<UserRoleTreeItemModel>();
    }
}
