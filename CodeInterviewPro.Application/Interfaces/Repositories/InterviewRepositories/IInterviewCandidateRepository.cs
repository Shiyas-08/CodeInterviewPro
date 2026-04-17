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

        Task<IEnumerable<InterviewCandidate>> GetByInterviewIdAsync(Guid interviewId, Guid tenantId);

        Task<InterviewCandidate?> GetAsync(Guid interviewId, Guid candidateId, Guid tenantId);
    }
}
