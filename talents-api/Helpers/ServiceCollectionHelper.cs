using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TalentsApi.Services;

namespace TalentsApi.Helpers
{
    public static class ServiceCollectionHelper
    {
        public static IServiceCollection AddServices(this IServiceCollection services)
        {
            services.AddHttpContextAccessor();
            services.AddScoped<UserService>();
            services.AddScoped<AccountService>();
            services.AddScoped<EmailService>();
            services.AddScoped<UserRoleService>();
            services.AddScoped<FilesService>();
            return services;
        }
    }
}
