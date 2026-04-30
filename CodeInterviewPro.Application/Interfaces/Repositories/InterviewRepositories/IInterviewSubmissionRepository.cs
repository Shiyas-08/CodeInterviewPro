using CodeInterviewPro.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeInterviewPro.Application.Interfaces.Repositories.InterviewRepositories
{
    public interface IInterviewSubmissionRepository
    {
        Task<Guid> CreateAsync(InterviewSubmission submission);
        Task<IEnumerable<InterviewSubmission>> GetByCandidateAsync(Guid candidateId);
        Task<IEnumerable<InterviewSubmission>> GetByInterviewAndCandidateAsync(Guid interviewId, Guid candidateId);
    }
}
