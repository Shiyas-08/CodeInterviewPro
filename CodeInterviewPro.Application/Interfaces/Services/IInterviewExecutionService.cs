using CodeInterviewPro.Application.DTOs.Interview;
using CodeInterviewPro.Domain.Entities;

namespace CodeInterviewPro.Application.Interfaces.Services
{
    public interface IInterviewExecutionService
    {
        Task<StartInterviewResponse> StartInterviewAsync(string token);

        Task<ExecutionResult> SubmitCodeAsync(
     SubmitCodeRequest request);
        Task<ExecutionResult> RunCodeAsync(
    SubmitCodeRequest request);
    }
}