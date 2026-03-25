using CodeInterviewPro.Application.DTOs;
using CodeInterviewPro.Application.Interfaces.Repositories;
using CodeInterviewPro.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeInterviewPro.Application.Security
{
    public class TenantService : ITenantService
    {
        private readonly ITenantRepository _repo;

        public TenantService(ITenantRepository repo)
        {
            _repo = repo;
        }

        public async Task CreateAsync(CreateTenantRequest request)
        {
            var nameExists = await _repo.GetByNameAsync(request.Name);
            var domainExists = await _repo.GetByDomainAsync(request.Domain);

            if (nameExists != null)
                throw new Exception("Tenant name already exists");

            if (domainExists != null)
                throw new Exception("Domain already exists");

            var tenant = new Tenant
            {
                Id = Guid.NewGuid(),
                Name = request.Name,
                Domain = request.Domain,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            await _repo.CreateAsync(tenant);
        }

        public async Task<IEnumerable<Tenant>> GetAllAsync()
        {
            return await _repo.GetAllAsync();
        }

        public async Task<Tenant> GetByIdAsync(Guid id)
        {
            return await _repo.GetByIdAsync(id);
        }

        public async Task UpdateAsync(Guid id, UpdateTenantRequest request)
        {
            var tenant = await _repo.GetByIdAsync(id);

            if (tenant == null)
                throw new Exception("Tenant not found");

            var nameExists = await _repo.GetByNameAsync(request.Name);
            var domainExists = await _repo.GetByDomainAsync(request.Domain);

            if (nameExists != null && nameExists.Id != id)
                throw new Exception("Tenant name already exists");

            if (domainExists != null && domainExists.Id != id)
                throw new Exception("Domain already exists");

            tenant.Name = request.Name;
            tenant.Domain = request.Domain;
            tenant.IsActive = request.IsActive;
            tenant.UpdatedAt = DateTime.UtcNow;

            await _repo.UpdateAsync(tenant);
        }

        public async Task DeleteAsync(Guid id)
        {
            var tenant = await _repo.GetByIdAsync(id);

            if (tenant == null)
                throw new Exception("Tenant not found");

            tenant.IsActive = false;
            tenant.UpdatedAt = DateTime.UtcNow;

            await _repo.UpdateAsync(tenant);
        }
    }
}
