using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace TalentsApi.Models.Entities
{
    [Table("User")]
    public class User
    {
        [Key]
        public int Id { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public bool IsLocked { get; set; }
        public bool IsEmailVerified { get; set; }
        public DateTime? LastLogin { get; set; }
        public DateTime RegistrationDate { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public int CompanyId { get; set; }
        public string EmailVerificationToken { get; set; }
        public DateTime? EmailVerificationExpiration { get; set; }
        public string ResetPasswordToken { get; set;}
        public DateTime? ResetPasswordExpiration { get; set; }
        // public int RoleId { get; set; }
        public bool ShouldSetPasswordOnNextLogin { get; set; }
        public bool IsLockOutEnabled { get; set; }
        public string SpecialPermissions { get; set; }
        public bool IsActive { get; set; }
        public bool EmailVerificationRequired { get; set; }
        public string Picture { get; set; }
    }
}
