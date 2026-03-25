using CodeInterviewPro.Application.Interfaces;
using CodeInterviewPro.Application.Security;
using CodeInterviewPro.Application.Services;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeInterviewPro.Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            //register services 
            services.AddScoped<IAdminService, AdminService>();
            services.AddScoped<ITenantService, TenantService>();
            return services;
        }
    }
}
