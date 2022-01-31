using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TalentsApi.Models.Auth
{
    public class UserClaims
    {
        public List<string> Permissions { get; set; } = new List<string>();
        public int UserId { get; set; }
        public int CompanyId { get; set; }
    }
}
