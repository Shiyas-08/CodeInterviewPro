using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeInterviewPro.Application.DTOs.Interview
{
    public class CreateQuestionDto
    {
        public string Title { get; set; } = default!;
        public string Description { get; set; } = default!;
        public string? StarterCode { get; set; }
        public string TestCases { get; set; } = default!;
        public string Language { get; set; } = default!;
    }
}
