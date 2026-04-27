using CodeInterviewPro.Application.Interfaces.Services;
using CodeInterviewPro.Infrastructure.CodeExecution.StaticAnalysis.Roslyn;
using System.Linq;
using System.Threading.Tasks;

namespace CodeInterviewPro.Infrastructure.CodeExecution.StaticAnalysis
{
    public class CSharpStaticAnalyzer : IStaticCodeAnalyzer
    {
        public Task<string> AnalyzeAsync(string code)
        {
            var roslyn = new RoslynAnalyzer();
            var errors = roslyn.Analyze(code);

            if (errors.Any())
            {
                return Task.FromResult(string.Join("\n", errors));
            }

            return Task.FromResult("");
        }
    }
}