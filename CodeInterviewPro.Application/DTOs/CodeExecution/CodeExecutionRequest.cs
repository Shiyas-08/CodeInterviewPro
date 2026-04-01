using CodeInterviewPro.Domain.Entities;
using CodeInterviewPro.Domain.Enums;

namespace CodeInterviewPro.Application.DTOs.CodeExecutionRequest
{
    public class CodeExecutionRequest
    {
        public string Code { get; set; }

        public ProgrammingLanguage Language { get; set; }

        public string MethodName { get; set; }

        public List<TestCase> TestCases { get; set; }
    }
}