using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TalentsApi.Models.User
{
    public class UserModel
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public bool IsActive { get; set; }
        public bool IsLocked { get; set; }
        public bool ShouldSetPasswordOnNextLogin { get; set; }
        public bool IsLockOutEnabled { get; set; }
        public List<string> Roles { get; set; } = new List<string>();
        public bool IsEmailVerified { get; set; }
        public string Picture { get; set; }
    }
}
