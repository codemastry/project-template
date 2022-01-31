using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Linq;
using TalentsApi.Models.Auth;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
public class AuthorizeAttribute : Attribute, IAuthorizationFilter
{
    private readonly string[] _allowedAccess;

    public AuthorizeAttribute()
    {

    }

    public AuthorizeAttribute(params string[] access)
    {
        _allowedAccess = access;
    }

    public void OnAuthorization(AuthorizationFilterContext context)
    {
        var user = (UserClaims)context.HttpContext.Items["UserClaims"];
        if (user == null)
        {
            // not logged in
            context.Result = new JsonResult(new { message = "Unauthorized" }) { StatusCode = StatusCodes.Status401Unauthorized };
            return;
        }

        if (_allowedAccess?.Length > 0)
        {
            var allowed = false;
            foreach (var access in _allowedAccess)
            {
                var foundInClaims = user.Permissions.FirstOrDefault(m => m == access);
                if (!string.IsNullOrWhiteSpace(foundInClaims))
                {
                    allowed = true;
                    break;
                }
            }

            if (!allowed)
                context.Result = new JsonResult(new { message = "Unauthorized" }) { StatusCode = StatusCodes.Status401Unauthorized };
        }
    }
}