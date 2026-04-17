using System.Text;
using CodeInterviewPro.Application.Interfaces.Services;
using CodeInterviewPro.Domain.Entities;

namespace CodeInterviewPro.Infrastructure.CodeExecution.Templates
{
    public class JavaRunnerTemplate : ICodeRunnerTemplate
    {
        public string WrapCode(
            string code,
            List<TestCase> testCases,
            string methodName)
        {
            var sb = new StringBuilder();

            sb.AppendLine("public class Main {");
            sb.AppendLine();

            sb.AppendLine(code);
            sb.AppendLine();

            sb.AppendLine("public static void main(String[] args) {");

            int index = 0;

            foreach (var test in testCases)
            {
                sb.AppendLine(
                    $"System.out.println(\"RESULT_{index}:\" + {methodName}({test.Input}));");

                index++;
            }

            sb.AppendLine("}");
            sb.AppendLine("}");

            return sb.ToString();
        }
    }
}