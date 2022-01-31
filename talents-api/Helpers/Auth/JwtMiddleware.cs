using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TalentsApi.Access;
using TalentsApi.DataAccess;
using TalentsApi.Models.AppSettings;
using TalentsApi.Models.Auth;
using TalentsApi.Services;

namespace TalentsApi.Helpers.Auth
{
    public class JwtMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly AppSettingsModel _appSettingsModel;
        private readonly IServiceProvider _serviceProvider;

        public JwtMiddleware(RequestDelegate next,
            AppSettingsModel appSettingsModel,
            IServiceProvider serviceProvider)
        {
            _next = next;
            _appSettingsModel = appSettingsModel;
            _serviceProvider = serviceProvider;
        }

        public async Task Invoke(HttpContext context)
        {
            var token = context.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();

            if (token != null)
                context = AttachUserToContext(context, token);

            await _next(context);
        }

        private HttpContext AttachUserToContext(HttpContext context, string token)
        {
            try
            {
                using (var scope = _serviceProvider.CreateScope())
                {
                    var _context = scope.ServiceProvider.GetRequiredService<TalentsDbContext>();
                    var tokenHandler = new JwtSecurityTokenHandler();
                    var key = Encoding.ASCII.GetBytes(_appSettingsModel.JwtConfig.Secret);
                    tokenHandler.ValidateToken(token, new TokenValidationParameters
                    {
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(key),
                        ValidateIssuer = false,
                        ValidateAudience = false,
                        // set clockskew to zero so tokens expire exactly at token expiration time (instead of 5 minutes later)
                        ClockSkew = TimeSpan.Zero
                    }, out SecurityToken validatedToken);

                    var jwtToken = (JwtSecurityToken)validatedToken;
                    var userId = jwtToken.Claims.First(x => x.Type == "userId").Value;

                    // attach user to context on successful jwt validation
                    var user = _context.Users.FirstOrDefault(m => m.Id == Convert.ToInt32(userId));
                    if (user != null)
                    {
                        var userRoleIds = _context.UserRoleIds.Where(m => m.UserId == user.Id).Select(b => b.RoleId).ToList();
                        var roles = _context.UserRoles.Where(m => userRoleIds.Any(a => a == m.Id)).ToList();

                        var tempPermissions = new List<string>();
                        if (!string.IsNullOrWhiteSpace(user.SpecialPermissions))
                            tempPermissions.AddRange(JsonConvert.DeserializeObject<List<string>>(user.SpecialPermissions));

                        foreach (var role in roles)
                        {
                            if (!string.IsNullOrWhiteSpace(role.Permission))
                                tempPermissions.AddRange(JsonConvert.DeserializeObject<List<string>>(role.Permission));
                        }

                        // attach parent permissions
                        var allPermissions = PermissionList.Permissions;
                        var resultPermissions = new List<string>();
                        resultPermissions.AddRange(tempPermissions);
                        foreach (var p in tempPermissions)
                        {
                            var permission = allPermissions.FirstOrDefault(m => m.Id == p);
                            resultPermissions.AddRange(JwtHelper.GetPermissionParents(permission));
                        }

                        context.Items["UserClaims"] = new UserClaims
                        {
                            UserId = user.Id,
                            CompanyId = user.CompanyId,
                            Permissions = resultPermissions.Distinct().ToList()
                        };
                    }
                }
            }
            catch (Exception e)
            {
                // do nothing if jwt validation fails
                // user is not attached to context so request won't have access to secure routes
            }
            return context;
        }
    }
}
