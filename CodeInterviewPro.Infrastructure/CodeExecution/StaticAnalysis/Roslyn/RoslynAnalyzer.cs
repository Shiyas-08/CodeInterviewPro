using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace CodeInterviewPro.Infrastructure.CodeExecution.StaticAnalysis.Roslyn
{
    public class RoslynAnalyzer
    {
        public List<string> Analyze(string code)
        {
            // Wrap the candidate's method snippet in a class so Roslyn
            // can parse it without false "object reference required" errors.
            bool needsWrapper = !code.TrimStart().StartsWith("using") &&
                                !code.Contains("class ") &&
                                !code.Contains("namespace ");

            string wrappedCode = needsWrapper
                ? $"using System;\nusing System.Collections.Generic;\npublic class __Wrapper__ {{\n{code}\n}}"
                : code;

            var syntaxTree = CSharpSyntaxTree.ParseText(wrappedCode);

            // Only check for SYNTAX parse errors (no full compilation needed)
            // This avoids false "object reference" / semantic errors on method snippets
            var syntaxDiagnostics = syntaxTree.GetDiagnostics();

            var result = new List<string>();

            foreach (var diagnostic in syntaxDiagnostics)
            {
                // Only report errors, never warnings
                if (diagnostic.Severity == DiagnosticSeverity.Error)
                {
                    result.Add(diagnostic.GetMessage());
                }
            }

            return result;
        }
    }
}