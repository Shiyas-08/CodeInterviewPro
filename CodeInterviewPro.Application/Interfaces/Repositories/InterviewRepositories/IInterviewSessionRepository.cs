using CodeInterviewPro.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeInterviewPro.Application.Interfaces.Repositories.InterviewsRepositories
{
    public interface IInterviewSessionRepository
    {
        Task CreateAsync(InterviewSession session);

        Task<InterviewSession?> GetAsync(long interviewId, long candidateId, long tenantId);

        Task UpdateAsync(InterviewSession session);
    }
}
