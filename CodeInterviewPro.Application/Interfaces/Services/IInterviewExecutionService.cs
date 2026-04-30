using CodeInterviewPro.Application.DTOs.Interview;
using CodeInterviewPro.Domain.Entities;

namespace CodeInterviewPro.Application.Interfaces.Services
{
    public interface IInterviewExecutionService
    {
        Task<StartInterviewResponse> StartInterviewAsync(string token);
        Task<ExecutionResult> RunCodeAsync(SubmitCodeRequest request);
        Task<ExecutionResult> SubmitCodeAsync(
     SubmitCodeRequest request);
    }
}