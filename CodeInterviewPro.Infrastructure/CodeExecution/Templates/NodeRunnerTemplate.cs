using System.Text;
using CodeInterviewPro.Application.Interfaces.Services;
using CodeInterviewPro.Domain.Entities;

namespace CodeInterviewPro.Infrastructure.CodeExecution.Templates
{
    public class NodeRunnerTemplate : ICodeRunnerTemplate
    {
        public string WrapCode(
            string code,
            List<TestCase> testCases,
            string methodName)
        {
            var sb = new StringBuilder();

            sb.AppendLine(code);
            sb.AppendLine("");

            int index = 0;

            foreach (var test in testCases)
            {
                sb.AppendLine(
                    $"console.log(\"RESULT_{index}:\" + {methodName}({FormatInput(test.Input)}));");

                index++;
            }

            return sb.ToString();
        }

        private string FormatInput(string input)
        {
            if (int.TryParse(input, out _))
                return input;

            return $"\"{input}\"";
        }
    }
}