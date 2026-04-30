using System.Text;
using CodeInterviewPro.Application.Interfaces.Services;
using CodeInterviewPro.Domain.Entities;

namespace CodeInterviewPro.Infrastructure.CodeExecution.Templates
{
    public class CSharpRunnerTemplate : ICodeRunnerTemplate
    {
        public string WrapCode(
            string code,
            List<TestCase> testCases,
            string methodName)
        {
            var sb = new StringBuilder();

            sb.AppendLine("using System;");
            sb.AppendLine("");

            bool hasProgramClass = code.Contains("class Program");

            // If user already provided Program class, don't wrap again
            if (!hasProgramClass)
            {
                sb.AppendLine("public class Program");
                sb.AppendLine("{");
            }

            // user code
            sb.AppendLine(code);
            sb.AppendLine("");

            // Only add Main if not already exists
            if (!code.Contains("Main("))
            {
                sb.AppendLine("public static void Main(string[] args)");
                sb.AppendLine("{");

                int index = 0;

                foreach (var test in testCases)
                {
                    sb.AppendLine(
                        $"Console.WriteLine(\"RESULT_{index}:\" + {methodName}({test.Input}));");

                    index++;
                }

                sb.AppendLine("}");
            }

            if (!hasProgramClass)
            {
                sb.AppendLine("}");
            }

            var result = sb.ToString();

            Console.WriteLine("========== GENERATED CODE ==========");
            Console.WriteLine(result);
            Console.WriteLine("====================================");

            return result;
        }
    }
}

//using System.Text;
//using CodeInterviewPro.Application.Interfaces.Services;
//using CodeInterviewPro.Domain.Entities;

//namespace CodeInterviewPro.Infrastructure.CodeExecution.Templates
//{
//    public class CSharpRunnerTemplate : ICodeRunnerTemplate
//    {
//        public string WrapCode(
//            string code,
//            List<TestCase> testCases,
//            string methodName)
//        {
//            var sb = new StringBuilder();

//            sb.AppendLine("using System;");
//            sb.AppendLine("");

//            sb.AppendLine("public class Program");
//            sb.AppendLine("{");

//            // user method
//            sb.AppendLine(code);
//            sb.AppendLine("");

//            // FIXED Main Signature
//            sb.AppendLine("public static void Main(string[] args)");
//            sb.AppendLine("{");

//            int index = 0;

//            foreach (var test in testCases)
//            {
//                sb.AppendLine(
//                    $"Console.WriteLine(\"RESULT_{index}:\" + {methodName}({FormatInput(test.Input)}));");

//                index++;
//            }

//            sb.AppendLine("}");
//            sb.AppendLine("}");

//            var result = sb.ToString();

//            Console.WriteLine("========== GENERATED CODE ==========");
//            Console.WriteLine(result);
//            Console.WriteLine("====================================");

//            return result;
//        }

//        private string FormatInput(string input)
//        {
//            if (int.TryParse(input, out _))
//                return input;

//            if (double.TryParse(input, out _))
//                return input;

//            if (bool.TryParse(input, out _))
//                return input.ToLower();

//            return $"\"{input}\"";
//        }
//    }
//}