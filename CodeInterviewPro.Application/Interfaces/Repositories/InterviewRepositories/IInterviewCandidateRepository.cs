using CodeInterviewPro.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeInterviewPro.Application.Interfaces.Repositories.InterviewsRepositories
{
    public interface IInterviewCandidateRepository
    {
        Task AssignCandidateAsync(InterviewCandidate candidate);

        Task<IEnumerable<InterviewCandidate>> GetByInterviewIdAsync(long interviewId, Guid tenantId);

        Task<InterviewCandidate?> GetAsync(long interviewId, long candidateId, Guid tenantId);
    }
}
