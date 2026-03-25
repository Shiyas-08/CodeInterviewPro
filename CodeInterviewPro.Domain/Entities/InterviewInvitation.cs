using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeInterviewPro.Domain.Entities
{
    public class InterviewInvitation
    {
        public long Id { get; set; }

        public long TenantId { get; set; }

        public long InterviewId { get; set; }

        public long CandidateId { get; set; }

        public string Token { get; set; } = string.Empty;

        public DateTime ExpiryTime { get; set; }

        public bool IsUsed { get; set; }
    }
}
