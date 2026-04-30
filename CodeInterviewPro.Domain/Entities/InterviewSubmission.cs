using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeInterviewPro.Domain.Entities
{
    public class InterviewSubmission
    {
        public Guid Id { get; set; }

        public Guid InterviewId { get; set; }

        public Guid QuestionId { get; set; }

        public Guid CandidateId { get; set; }

        public Guid TenantId { get; set; }

        public string Language { get; set; } = string.Empty;

        public string Code { get; set; } = string.Empty;

        public DateTime SubmittedAt { get; set; }

        // Score is NOT stored here — see ExecutionHistory table for scores
    }
}
