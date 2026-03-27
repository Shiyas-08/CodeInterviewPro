using CodeInterviewPro.Application.DTOs.Interview;
using CodeInterviewPro.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeInterviewPro.Application.Interfaces.Services
{
    public interface IInterviewExecutionService
    {
        Task<JoinInterviewResponse> JoinInterviewAsync(string token);
        Task<StartInterviewResponse> StartInterviewAsync(string token);
        Task<List<GetQuestionsResponse>> GetQuestionsAsync(string token);
        Task<SubmitCodeResponse> SubmitCodeAsync(SubmitCodeRequest request);
    }
}
