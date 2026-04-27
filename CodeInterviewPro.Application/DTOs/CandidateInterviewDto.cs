using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeInterviewPro.Application.DTOs
{
    public class CandidateInterviewDto
    {
        public Guid InvitationId { get; set; }
        public string Token { get; set; }
        public bool IsUsed { get; set; }
        public DateTime ExpiryTime { get; set; }
        public DateTime? StartedAt { get; set; }

        public Guid TenantId { get; set; }   // ADD THIS

        public Guid InterviewId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public int DurationMinutes { get; set; }

        public string Status { get; set; }
    }
}
