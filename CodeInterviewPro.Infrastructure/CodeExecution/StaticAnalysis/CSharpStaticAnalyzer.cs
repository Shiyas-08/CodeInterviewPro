using CodeInterviewPro.Application.Interfaces.Services;

namespace CodeInterviewPro.Infrastructure.CodeExecution.StaticAnalysis
{
    public class CSharpStaticAnalyzer : IStaticCodeAnalyzer
    {
        public async Task<string> AnalyzeAsync(string code)
        {
            // TODO: Integrate Roslyn analyzer here
            return "";
        }
    }
}