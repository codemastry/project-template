using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using TalentsApi.Access;
using TalentsApi.Models.Entities;

namespace TalentsApi.Helpers.Auth
{
    public static class JwtHelper
    {
        public static string GenerateJwt(User user, string secret, double expirationDays)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(secret);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim("userId", user.Id.ToString()),
                    //new Claim("companyId", user.CompanyId.ToString()),
                    //new Claim("permissions", role.Permission),
                }),
                Expires = DateTime.UtcNow.AddDays(expirationDays),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        public static List<string> GetPermissionParents(Permission permission)
        {
            var result = new List<string>();
            if (!string.IsNullOrWhiteSpace(permission.ParentId))
            {
                result.Add(permission.ParentId);
                var parentPermission = PermissionList.Permissions.FirstOrDefault(m => m.Id == permission.ParentId);
                if (parentPermission != null)
                    result.AddRange(GetPermissionParents(parentPermission));
            }
            return result;
        }
    }
}
