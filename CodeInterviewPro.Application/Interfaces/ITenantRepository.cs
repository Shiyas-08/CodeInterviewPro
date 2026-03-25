using CodeInterviewPro.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeInterviewPro.Application.Interfaces
{
    public interface ITenantRepository
    {
        Task CreateAsync(Tenant tenant);
        Task<IEnumerable<Tenant>> GetAllAsync();
        Task<Tenant> GetByIdAsync(Guid id);
        Task<Tenant> GetByNameAsync(string name);
        Task<Tenant> GetByDomainAsync(string domain);
        Task UpdateAsync(Tenant tenant);
    }
}
