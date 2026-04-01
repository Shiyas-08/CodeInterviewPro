using CodeInterviewPro.Domain.Entities;
using System.Collections.Generic;
using System.Text;
using CodeInterviewPro.Application.Interfaces.Services;
namespace CodeInterviewPro.Infrastructure.CodeExecution.Templates
{
    public class CSharpRunnerTemplate : ICodeRunnerTemplate
    {
        public string WrapCode(
            string userCode,
            List<TestCase> testCases,
            string methodName)
        {
            var sb = new StringBuilder();

            sb.AppendLine("using System;");
            sb.AppendLine("using System.Linq;");
            sb.AppendLine("using System.Collections.Generic;");

            sb.AppendLine(userCode);

            sb.AppendLine("public class Runner");
            sb.AppendLine("{");
            sb.AppendLine("public static void Main()");
            sb.AppendLine("{");

            for (int i = 0; i < testCases.Count; i++)
            {
                var test = testCases[i];

                sb.AppendLine($@"
try
{{
var result = Solution.{methodName}(""{test.Input}"");

if(result.ToString() == ""{test.ExpectedOutput}"")
Console.WriteLine(""TEST_{i}_PASS"");
else
Console.WriteLine(""TEST_{i}_FAIL"");
}}
catch(Exception)
{{
Console.WriteLine(""TEST_{i}_ERROR"");
}}
");
            }

            sb.AppendLine("}");
            sb.AppendLine("}");

            return sb.ToString();
        }
    }
}