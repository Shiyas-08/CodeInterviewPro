using CodeInterviewPro.Application.DTOs;
using CodeInterviewPro.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeInterviewPro.Application.Interfaces
{
    public interface ITenantService
    {
        Task CreateAsync(CreateTenantRequest request);
        Task<IEnumerable<Tenant>> GetAllAsync();
        Task<Tenant> GetByIdAsync(Guid id);
        Task UpdateAsync(Guid id, UpdateTenantRequest request);
        Task DeleteAsync(Guid id);
    }
}
