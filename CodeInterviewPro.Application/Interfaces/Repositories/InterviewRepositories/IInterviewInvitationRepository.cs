using CodeInterviewPro.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeInterviewPro.Application.Interfaces.Repositories.InterviewsRepositories
{
    public interface IInterviewInvitationRepository
    {
        Task CreateAsync(InterviewInvitation invitation);

        Task<InterviewInvitation?> GetByTokenAsync(string token);

        Task MarkUsedAsync(long id);
        Task UpdateAsync(InterviewInvitation invitation);
        Task UpdateCandidateAsync(string token, Guid candidateId);
     }
}
