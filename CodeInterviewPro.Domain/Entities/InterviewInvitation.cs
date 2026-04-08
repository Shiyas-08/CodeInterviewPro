using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeInterviewPro.Domain.Entities
{
    public class InterviewInvitation
    {
        public Guid Id { get; set; }

        public Guid TenantId { get; set; }

        public Guid InterviewId { get; set; }

        public Guid CandidateId { get; set; }

        public string Token { get; set; } = string.Empty;

        public DateTime ExpiryTime { get; set; }

        public bool IsUsed { get; set; }
        public DateTime? StartedAt { get; set; }
    }
}
