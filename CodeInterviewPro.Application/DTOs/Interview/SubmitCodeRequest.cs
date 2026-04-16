using CodeInterviewPro.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeInterviewPro.Application.DTOs.Interview
{
    public class SubmitCodeRequest
    {
        public string Token { get; set; } = string.Empty;

        public Guid QuestionId { get; set; }

        public string Code { get; set; } = string.Empty;

        public ProgrammingLanguage Language { get; set; }

    }


}
