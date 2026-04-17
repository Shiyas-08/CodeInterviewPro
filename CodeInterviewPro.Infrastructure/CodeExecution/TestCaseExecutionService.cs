using System.Threading;
using CodeInterviewPro.Domain.Entities;
using CodeInterviewPro.Domain.Enums;

namespace CodeInterviewPro.Infrastructure.CodeExecution
{
    public class TestCaseExecutionService
    {
        private readonly MultiLanguageExecutionService _executionService;

        public TestCaseExecutionService(
            MultiLanguageExecutionService executionService)
        {
            _executionService = executionService;
        }

        public async Task<List<TestCaseResult>> ExecuteAsync(
            string code,
            ProgrammingLanguage language,
            List<TestCase> testCases,
            string methodName,
            CancellationToken token)
        {
            token.ThrowIfCancellationRequested();

            var results = new List<TestCaseResult>();

            var output =
                await _executionService.ExecuteAsync(
                    code,
                    language,
                    testCases,
                    methodName);

            token.ThrowIfCancellationRequested();

            // DEBUG: Raw Output
            Console.WriteLine("========== RAW OUTPUT ==========");
            Console.WriteLine(output);
            Console.WriteLine("================================");

            var lines =
                output
                .Split(new[] { "\r\n", "\n" },
                StringSplitOptions.RemoveEmptyEntries)
                .Select(x => x.Trim())
                .Where(x => x.Contains("RESULT_"))
                .ToList();

            // DEBUG: Parsed Lines
            Console.WriteLine("========== PARSED LINES ==========");
            foreach (var line in lines)
            {
                Console.WriteLine(line);
            }
            Console.WriteLine("=================================");

            for (int i = 0; i < testCases.Count; i++)
            {
                token.ThrowIfCancellationRequested();

                var expected =
                    Normalize(testCases[i].ExpectedOutput);

                var actual =
                    i < lines.Count
                        ? Normalize(
                            lines[i]
                                .Substring(
                                    lines[i].IndexOf(':') + 1)
                                .Trim())
                        : string.Empty;

                // DEBUG: Compare
                Console.WriteLine($"TEST CASE {i}");
                Console.WriteLine($"Expected: {expected}");
                Console.WriteLine($"Actual  : {actual}");

                var passed =
                    string.Equals(
                        actual,
                        expected,
                        StringComparison.OrdinalIgnoreCase);

                Console.WriteLine($"Passed  : {passed}");
                Console.WriteLine("--------------------------------");

                results.Add(new TestCaseResult
                {
                    Output = actual,
                    Expected = expected,
                    Passed = passed
                });
            }

            return results;
        }

        private static string Normalize(string? value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return string.Empty;

            return value
                .Trim()
                .Replace("\r", "")
                .Replace("\n", "")
                .Replace(" ", "");
        }
    }
}