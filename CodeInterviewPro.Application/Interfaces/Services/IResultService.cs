using CodeInterviewPro.Application.DTOs.Result;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeInterviewPro.Application.Interfaces.Services
{
    public interface IResultService
    {
        Task<InterviewResultDto> GetMyResultAsync(Guid interviewId);
        Task<InterviewResultDto> GetCandidateResultAsync(Guid candidateId, Guid interviewId);
    }
}
