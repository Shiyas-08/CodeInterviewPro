using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace CodeInterviewPro.Infrastructure.CodeExecution.StaticAnalysis.Roslyn
{
    public class RoslynAnalyzer : IStaticAnalyzer
    {
        public bool Supports(CodeInterviewPro.Domain.Enums.ProgrammingLanguage language)
        {
            return language == CodeInterviewPro.Domain.Enums.ProgrammingLanguage.CSharp;
        }

        public Task<string> AnalyzeAsync(string code)
        {
            var errors = Analyze(code);
            return Task.FromResult(string.Join("\n", errors));
        }

        public List<string> Analyze(string code)
        {
            if (string.IsNullOrWhiteSpace(code))
                return new List<string>();

            // Wrap the candidate's method snippet in a class so Roslyn
            // can parse it without false "object reference required" errors.
            bool needsWrapper = !code.TrimStart().StartsWith("using") &&
                                !code.Contains("class ") &&
                                !code.Contains("namespace ");

            string wrappedCode = needsWrapper
                ? $"using System;\nusing System.Collections.Generic;\npublic class __Wrapper__ {{\n{code}\n}}"
                : code;

            var options = CSharpParseOptions.Default.WithLanguageVersion(LanguageVersion.Latest);
            var syntaxTree = CSharpSyntaxTree.ParseText(wrappedCode, options);

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