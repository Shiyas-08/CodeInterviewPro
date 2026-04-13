using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using System.Reflection;

namespace CodeInterviewPro.Infrastructure.CodeExecution.StaticAnalysis.Roslyn
{
    public class RoslynAnalyzer
    {
        public List<string> Analyze(string code)
        {                                                                                                                                       
            var syntaxTree =
                CSharpSyntaxTree.ParseText(code);

            var references =
                AppDomain.CurrentDomain
                .GetAssemblies()
                .Where(a => !a.IsDynamic &&
                            !string.IsNullOrEmpty(a.Location))
                .Select(a =>
                    MetadataReference.CreateFromFile(a.Location))
                .Cast<MetadataReference>()
                .ToList();

            var compilation =
                CSharpCompilation.Create(
                    "Analysis",
                    new[] { syntaxTree },
                    references,
                    new CSharpCompilationOptions(
                        OutputKind.ConsoleApplication));

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