using System.Threading.Tasks;

namespace CodeInterviewPro.Infrastructure.CodeExecution
{
    public interface IStaticAnalyzer
    {
        bool Supports(CodeInterviewPro.Domain.Enums.ProgrammingLanguage language);
        Task<string> AnalyzeAsync(string code);
    }
}
