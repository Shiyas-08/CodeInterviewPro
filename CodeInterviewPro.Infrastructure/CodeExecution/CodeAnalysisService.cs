using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using System.Runtime.Loader;

namespace CodeInterviewPro.Infrastructure.CodeExecution
{
    public class CodeAnalysisService
    {
        public List<string> Analyze(string code)
        {
            var wrappedCode = $@"
using System;

class Program
{{
    static void Main()
    {{
        {code}
    }}
}}";

            var syntaxTree =
                CSharpSyntaxTree.ParseText(wrappedCode);

            var references =
                ((string)AppContext.GetData("TRUSTED_PLATFORM_ASSEMBLIES"))
                .Split(Path.PathSeparator)
                .Select(p => MetadataReference.CreateFromFile(p));

            var compilation =
                CSharpCompilation.Create(
                    "Analysis",
                    new[] { syntaxTree },
                    references,
                    new CSharpCompilationOptions(
                        OutputKind.ConsoleApplication));

            var diagnostics =
                compilation.GetDiagnostics();

            return diagnostics
                .Where(d =>
                    d.Severity == DiagnosticSeverity.Error)
                .Select(d => d.ToString())
                .ToList();
        }
    }
}