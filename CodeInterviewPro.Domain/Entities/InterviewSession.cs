using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace CodeInterviewPro.Domain.Entities
{
    public class InterviewSession
    {
        public long Id { get; set; }

        public Guid TenantId { get; set; }

        public long InterviewId { get; set; }

        public long CandidateId { get; set; }

        public string Token { get; set; }

        public DateTime? StartTime { get; set; }

        public DateTime? EndTime { get; set; }

        public int DurationMinutes { get; set; }

        public int? RemainingSeconds { get; set; }

        public int Status { get; set; }

        public bool IsActive { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}