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
        Task<long> CreateAsync(InterviewSession session);

        Task<InterviewSession?> GetByTokenAsync(string token);

        Task UpdateAsync(InterviewSession session);
    }
}
