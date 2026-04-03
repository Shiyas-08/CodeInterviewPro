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
            string methodName)
        {
            var tasks = testCases.Select(async testCase =>
            {
                try
                {
                    var output =
                        await _executionService.ExecuteAsync(
                            code,
                            language,
                            new List<TestCase> { testCase },
                            methodName);

                    return new TestCaseResult
                    {
                        Output = output?.Trim(),
                        Expected = testCase.ExpectedOutput?.Trim(),
                        Passed =
                            output?.Trim() ==
                            testCase.ExpectedOutput?.Trim()
                    };
                }
                catch (Exception ex)
                {
                    return new TestCaseResult
                    {
                        Output = ex.Message,
                        Expected = testCase.ExpectedOutput,
                        Passed = false
                    };
                }
            });

            var results =
                await Task.WhenAll(tasks);

            return results.ToList();
        }
    }
}