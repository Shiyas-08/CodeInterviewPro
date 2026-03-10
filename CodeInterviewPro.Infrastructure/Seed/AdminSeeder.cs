using CodeInterviewPro.Application.Interfaces;
using CodeInterviewPro.Domain.Entities;
using CodeInterviewPro.Domain.Enums;
using CodeInterviewPro.Infrastructure.Services;
using Microsoft.Extensions.DependencyInjection;

namespace CodeInterviewPro.Infrastructure.Seed
{
    public class AdminSeeder
    {
        public static async Task Seed(IServiceProvider services)
        {
            var userRepo = services.GetRequiredService<IUserRepository>();
            var hasher = services.GetRequiredService<IPasswordHasher>();

            var admin = await userRepo.GetByEmail("admin@system.com");

            if (admin != null)
                return;

            var superAdmin = new User
            {
                TenantId = null,
                Email = "admin@system.com",
                PasswordHash = hasher.Hash("Admin@123"),
                FullName = "System Administrator",
                Role = UserRole.SuperAdmin
            };

            await userRepo.Create(superAdmin);
        }
    }
}