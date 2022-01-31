using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TalentsApi.Models.UserRole
{
    public class UserRoleModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int CompanyId { get; set; }
        public bool IsDefault { get; set; }
        public DateTime DateCreated { get; set; }
        public string Permission { get; set; }
        public bool IsAdmin { get; set; }
        public List<string> PermissionList
        {
            get
            {
                if (!string.IsNullOrWhiteSpace(Permission))
                    return JsonConvert.DeserializeObject<List<string>>(Permission);

                return new List<string>();
            }
        }
    }
}
