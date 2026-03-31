using CodeInterviewPro.Domain.Entities;
using CodeInterviewPro.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeInterviewPro.Application.DTOs.CodeExecution
{
    public class TestCaseRequest
    {
        public string Code { get; set; }

        public ProgrammingLanguage Language { get; set; }

        public List<TestCase> TestCases { get; set; }
    }
}
