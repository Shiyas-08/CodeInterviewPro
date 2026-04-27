using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CodeInterviewPro.Domain.Entities;
namespace CodeInterviewPro.Application.Interfaces.Repositories.InterviewRepositories
{
 

    public interface IQuestionRepository
    {
        Task<Guid> CreateAsync(Question question);

        Task<Question?> GetByIdAsync(Guid id, Guid tenantId);

        Task<IEnumerable<Question>> GetAllAsync(Guid tenantId);

        Task UpdateAsync(Question question);

        Task DeleteAsync(Guid id, Guid tenantId);
        Task<IEnumerable<Question>> GetByInterviewIdAsync(Guid interviewId);
    }
}
