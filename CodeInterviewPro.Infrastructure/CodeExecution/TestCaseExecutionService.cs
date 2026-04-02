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
                var output =
                    await _executionService.ExecuteAsync(
                        code,
                        language,
                        new List<TestCase> { testCase },
                        methodName);

                return new TestCaseResult
                {
                    Output = output.Trim(),
                    Expected = testCase.ExpectedOutput.Trim(),
                    Passed =
                        output.Trim() ==
                        testCase.ExpectedOutput.Trim()
                };
            });

            return (await Task.WhenAll(tasks)).ToList();
        }
    }
}