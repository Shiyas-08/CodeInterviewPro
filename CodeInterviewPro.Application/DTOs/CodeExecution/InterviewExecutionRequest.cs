using CodeInterviewPro.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeInterviewPro.Application.DTOs.CodeExecution
{
    public class InterviewExecutionRequest
    {
        public Guid InterviewId { get; set; }

        public Guid QuestionId { get; set; }

        public string Code { get; set; }

        public ProgrammingLanguage Language { get; set; }
    }
}
