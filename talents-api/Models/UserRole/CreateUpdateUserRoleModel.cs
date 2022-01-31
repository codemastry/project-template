using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace TalentsApi.Models.UserRole
{
    public class CreateUpdateUserRoleModel
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }
        public bool IsDefault { get; set; }
        public List<string> GrantedPermissions { get; set; }
    }
}
