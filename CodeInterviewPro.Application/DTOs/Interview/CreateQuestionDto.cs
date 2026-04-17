using CodeInterviewPro.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeInterviewPro.Application.DTOs.Interview
{
    public class CreateQuestionDto
    {
        public string Title { get; set; }

        public string Description { get; set; }

        public string? StarterCode { get; set; }

        public string TestCases { get; set; }

        public ProgrammingLanguage Language { get; set; }
        public string MethodName { get; set; }
    }
}
