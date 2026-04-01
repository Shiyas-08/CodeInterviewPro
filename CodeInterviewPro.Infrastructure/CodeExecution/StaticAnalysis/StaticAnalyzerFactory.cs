using CodeInterviewPro.Application.Interfaces.Services;
using CodeInterviewPro.Domain.Enums;

namespace CodeInterviewPro.Infrastructure.CodeExecution.StaticAnalysis
{
    public class StaticAnalyzerFactory
    {
        public static IStaticCodeAnalyzer GetAnalyzer(
            ProgrammingLanguage language)
        {
            return language switch
            {
                ProgrammingLanguage.CSharp
                    => new CSharpStaticAnalyzer(),

                ProgrammingLanguage.Python
                    => new PythonStaticAnalyzer(),

                ProgrammingLanguage.JavaScript
                    => new JavaScriptStaticAnalyzer(),

                ProgrammingLanguage.Java
                    => new JavaStaticAnalyzer(),

                ProgrammingLanguage.Go
                    => new GoStaticAnalyzer(),

                _ => throw new Exception("Unsupported language")
            };
        }
    }
}