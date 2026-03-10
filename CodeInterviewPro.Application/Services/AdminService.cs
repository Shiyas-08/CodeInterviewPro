using CodeInterviewPro.Application.DTOs;
using CodeInterviewPro.Application.Interfaces;
using CodeInterviewPro.Domain.Entities;
using CodeInterviewPro.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeInterviewPro.Application.Services
{
    public class AdminService : IAdminService
    {
        private readonly IUserRepository _repo;
        private readonly IPasswordHasher _hasher;

        public AdminService(
            IUserRepository repo,
            IPasswordHasher hasher)
        {
            _repo = repo;
            _hasher = hasher;
        }

        public async Task CreateHr(CreateHrRequest request)
        {
            var hash = _hasher.Hash(request.Password);

            var user = new User
            {
                TenantId = request.TenantId,
                Email = request.Email,
                PasswordHash = hash,
                FullName = request.FullName,
                Role = UserRole.HR
            };

            await _repo.Create(user);
        }
    }
}
