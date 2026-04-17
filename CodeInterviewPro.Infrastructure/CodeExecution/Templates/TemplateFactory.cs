using CodeInterviewPro.Application.Interfaces.Services;
using CodeInterviewPro.Domain.Enums;

namespace CodeInterviewPro.Infrastructure.CodeExecution.Templates
{
    public class TemplateFactory
    {
        public static ICodeRunnerTemplate GetTemplate(
            ProgrammingLanguage language)
        {
            return language switch
            {
                ProgrammingLanguage.CSharp =>
                    new CSharpRunnerTemplate(),

                ProgrammingLanguage.Python =>
                    new PythonRunnerTemplate(),

                ProgrammingLanguage.JavaScript =>
                    new NodeRunnerTemplate(),

                ProgrammingLanguage.Java =>
                    new JavaRunnerTemplate(),

                ProgrammingLanguage.Go =>
                    new GoRunnerTemplate(),

                _ => throw new Exception("Unsupported language")
            };
        }
    }
}