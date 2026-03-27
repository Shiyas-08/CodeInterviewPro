using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeInterviewPro.Domain.Entities
{
    public class InterviewSubmission
    {
        public long Id { get; set; }

        public long InterviewId { get; set; }

        public long QuestionId { get; set; }

        public long CandidateId { get; set; }

        public string Language { get; set; } = string.Empty;

        public string Code { get; set; } = string.Empty;

        public DateTime SubmittedAt { get; set; }
    }
}
