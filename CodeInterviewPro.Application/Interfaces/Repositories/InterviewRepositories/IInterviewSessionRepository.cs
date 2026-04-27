using CodeInterviewPro.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeInterviewPro.Application.Interfaces.Repositories.InterviewRepositories
{
    public interface IInterviewSessionRepository
    {
        Task CreateAsync(InterviewSession session);

        Task<InterviewSession?> GetByTokenAsync(string token);

        Task UpdateAsync(InterviewSession session);
    }
}
