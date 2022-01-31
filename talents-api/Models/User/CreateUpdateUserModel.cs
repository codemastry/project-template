using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace TalentsApi.Models.User
{
    public class CreateUpdateUserModel
    {
        public int Id { get; set; }

        [Required]
        public string FirstName { get; set; }

        [Required]
        public string LastName { get; set; }

        [Required]
        public string Email { get; set; }
        public string Password { get; set; }
        public bool IsActive { get; set; }
        public bool IsLocked { get; set; }
        public bool SetRandomPassword { get; set; }
        public bool ShouldSetPasswordOnNextLogin { get; set; }
        public bool SendActivationEmail { get; set; }
        public bool IsLockOutEnabled { get; set; }
        public bool EmailVerificationRequired { get; set; }
        public List<CreateUpdateUserRoleItem> Roles { get; set; } = new List<CreateUpdateUserRoleItem>();
        public string Picture { get; set; }
        public IFormFile UploadedPicture { get; set; }
    }
}

