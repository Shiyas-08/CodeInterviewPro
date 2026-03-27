using CodeInterviewPro.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeInterviewPro.Application.Interfaces.Services
{
    public interface IInterviewSessionService
    {
        Task<InterviewSession> StartSessionAsync(string token);

        Task<InterviewSession> GetSessionAsync(string token);

        Task EndSessionAsync(string token);
    }
}
