using CodeInterviewPro.Domain.Enums;

namespace CodeInterviewPro.Application.Interfaces.Services
{
    public interface IMethodNameDetectorService
    {
        string Detect(string candidateCode, string? starterCode, ProgrammingLanguage language);
    }
}
