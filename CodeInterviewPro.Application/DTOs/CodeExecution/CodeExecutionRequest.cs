using CodeInterviewPro.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeInterviewPro.Application.DTOs.CodeExecutionRequest
{
    public class CodeExecutionRequest
    {
        public string Code { get; set; }

        public ProgrammingLanguage Language { get; set; }

        public string Input { get; set; }
    }
}
