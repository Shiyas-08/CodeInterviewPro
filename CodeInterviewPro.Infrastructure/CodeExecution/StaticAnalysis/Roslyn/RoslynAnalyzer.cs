using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Diagnostics;

namespace CodeInterviewPro.Infrastructure.CodeExecution.StaticAnalysis.Roslyn
{
    public class RoslynAnalyzer
    {
        public List<string> Analyze(string code)
        {
            var tree =
                CSharpSyntaxTree.ParseText(code);

            var compilation =
                CSharpCompilation.Create(
                    "Analysis",
                    new[] { tree },
                    new[]
                    {
                        MetadataReference.CreateFromFile(
                            typeof(object).Assembly.Location)
                    });

            var diagnostics =
                compilation.GetDiagnostics();

            var result =
                new List<string>();

            foreach (var diagnostic in diagnostics)
            {
                if (diagnostic.Severity ==
                    DiagnosticSeverity.Error ||
                    diagnostic.Severity ==
                    DiagnosticSeverity.Warning)
                {
                    result.Add(
                        diagnostic.GetMessage());
                }
            }

            return result;
        }
    }
}