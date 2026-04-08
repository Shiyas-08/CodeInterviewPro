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

        public Guid InterviewId { get; set; }

        public Guid QuestionId { get; set; }

        public Guid CandidateId { get; set; }

        public string Language { get; set; } = string.Empty;

        public string Code { get; set; } = string.Empty;

        public DateTime SubmittedAt { get; set; }
    }
}
