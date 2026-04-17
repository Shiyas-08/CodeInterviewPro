using CodeInterviewPro.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeInterviewPro.Infrastructure.Repositories.InterviewRepositories
{
    public interface IInterviewRepository
    {
        Task<Guid> CreateAsync(Interview interview);

        Task<Interview> GetByIdAsync(Guid id, Guid tenantId);

        Task<IEnumerable<Interview>> GetAllAsync(Guid tenantId);

        Task UpdateAsync(Interview interview);
    }
}
