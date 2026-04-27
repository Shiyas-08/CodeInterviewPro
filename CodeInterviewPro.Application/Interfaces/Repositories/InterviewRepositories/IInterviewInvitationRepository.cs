using CodeInterviewPro.Application.DTOs;
using CodeInterviewPro.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeInterviewPro.Application.Interfaces.Repositories.InterviewRepositories
{
    public interface IInterviewInvitationRepository
    {
        Task CreateAsync(InterviewInvitation invitation);

        Task<InterviewInvitation?> GetByTokenAsync(string token);

        Task MarkUsedAsync(long id);
        Task UpdateAsync(InterviewInvitation invitation);
        Task UpdateCandidateAsync(string token, Guid candidateId);
        Task<IEnumerable<CandidateInterviewDto>> GetByCandidateIdAsync(Guid candidateId);
            Task BindInvitesByEmail(string email, Guid userId);
     }
}
