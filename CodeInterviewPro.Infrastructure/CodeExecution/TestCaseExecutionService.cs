using CodeInterviewPro.Domain.Entities;
using CodeInterviewPro.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
            List<TestCase> testCases)
        {
            var results = new List<TestCaseResult>();

            foreach (var testCase in testCases)
            {
                var output =
                    await _executionService.ExecuteAsync(
                        code,
                        language);

                var result = new TestCaseResult
                {
                    Output = output.Trim(),
                    Expected = testCase.ExpectedOutput.Trim(),
                    Passed =
                        output.Trim() ==
                        testCase.ExpectedOutput.Trim()
                };

                results.Add(result);
            }

            return results;
        }
    }
}