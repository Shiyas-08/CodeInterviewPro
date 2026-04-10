using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeInterviewPro.Domain.Entities
{
    public class CodeBertResult
    {
        public double Score { get; set; }

        public string Feedback { get; set; } = string.Empty;

        public string Complexity { get; set; } = string.Empty;

        public string Pattern { get; set; } = string.Empty;

        public string Suggestions { get; set; } = string.Empty;

        public string Algorithm { get; set; } = string.Empty;

        public string Readability { get; set; } = string.Empty;

        public string Maintainability { get; set; } = string.Empty;
    }
}
