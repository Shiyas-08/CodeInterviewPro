using System.Text;
using CodeInterviewPro.Application.Interfaces.Services;
using CodeInterviewPro.Domain.Entities;

namespace CodeInterviewPro.Infrastructure.CodeExecution.Templates
{
    public class GoRunnerTemplate : ICodeRunnerTemplate
    {
        public string WrapCode(
            string code,
            List<TestCase> testCases,
            string methodName)
        {
            var sb = new StringBuilder();

            sb.AppendLine("package main");
            sb.AppendLine("import \"fmt\"");
            sb.AppendLine();

            // user function
            sb.AppendLine(code);
            sb.AppendLine();

            // main
            sb.AppendLine("func main() {");

            int index = 0;

            foreach (var test in testCases)
            {
                sb.AppendLine(
                    $"fmt.Println(\"RESULT_{index}:\", {methodName}({test.Input}))");

                index++;
            }

            sb.AppendLine("}");

            return sb.ToString();
        }
    }
}