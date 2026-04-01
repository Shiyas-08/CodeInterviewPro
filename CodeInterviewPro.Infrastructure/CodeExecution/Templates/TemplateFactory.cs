using CodeInterviewPro.Application.Interfaces.Services;

namespace CodeInterviewPro.Infrastructure.CodeExecution.Templates
{
    public class TemplateFactory
    {
        public static ICodeRunnerTemplate GetTemplate(string language)
        {
            return language.ToLower() switch
            {
                "csharp" => new CSharpRunnerTemplate(),

                // We'll add later
                // "python" => new PythonRunnerTemplate(),
                // "javascript" => new NodeRunnerTemplate(),
                // "java" => new JavaRunnerTemplate(),
                // "go" => new GoRunnerTemplate(),

                _ => throw new Exception("Unsupported language")
            };
        }
    }
}