using CodeInterviewPro.Application.DTOs.Interview;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeInterviewPro.Application.Interfaces.Services
{
    public interface IInterviewService
    {
        Task<long> CreateAsync(CreateInterviewDto dto);

        Task AssignCandidateAsync(long interviewId, AssignCandidateDto dto);

        Task ScheduleAsync(long interviewId, ScheduleInterviewDto dto);

        Task<string> GenerateLinkAsync(long interviewId, GenerateLinkDto dto);
    }
}
