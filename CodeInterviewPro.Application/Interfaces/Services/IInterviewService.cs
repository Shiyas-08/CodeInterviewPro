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
        Task<Guid> CreateAsync(CreateInterviewDto dto);

        Task AssignCandidateAsync(Guid interviewId, AssignCandidateDto dto);

        Task ScheduleAsync(Guid interviewId, ScheduleInterviewDto dto);

        //Task<GenerateLinkResponse> GenerateLinkAsync(Guid interviewId, GenerateLinkDto dto);
        Task AssignQuestionsAsync(Guid interviewId,AssignQuestionsDto dto);
        Task<string> InviteCandidateAsync(Guid interviewId, InviteCandidateDto dto);
    }
}
