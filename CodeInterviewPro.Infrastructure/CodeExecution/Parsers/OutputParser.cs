using CodeInterviewPro.Application.Interfaces.Services;
using CodeInterviewPro.Domain.Entities;

namespace CodeInterviewPro.Infrastructure.CodeExecution.Parsers
{
    public class OutputParser : IOutputParser
    {
        public List<TestCaseResult> Parse(string output)
        {
            var results = new List<TestCaseResult>();

            var lines = output.Split('\n');

            foreach (var line in lines)
            {
                if (line.Contains("TEST_"))
                {
                    var passed = line.Contains("PASS");

                    results.Add(new TestCaseResult
                    {
                        Passed = passed,
                        Output = line,
                        Expected = ""
                    });
                }
            }

            return results;
        }
    }
}