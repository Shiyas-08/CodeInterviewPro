using CodeInterviewPro.Application.Interfaces;
using CodeInterviewPro.Infrastructure.Repositories;
using CodeInterviewPro.Infrastructure.Services;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeInterviewPro.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services)
        {
            //register services 
            services.AddScoped<DapperContext>();
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IPasswordHasher,PasswordHasher>();
            services.AddScoped<JwtService>();
            services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();
            services.AddScoped<ITenantRepository, TenantRepository>();
            return services;


        }
    }
}
