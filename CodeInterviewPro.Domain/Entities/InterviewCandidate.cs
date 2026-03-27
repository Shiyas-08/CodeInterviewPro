using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeInterviewPro.Domain.Entities
{
    public class InterviewCandidate
    {
        public long Id { get; set; }

        public Guid TenantId { get; set; }

        public long InterviewId { get; set; }

        public long CandidateId { get; set; }

        public int Status { get; set; }

        public DateTime AssignedAt { get; set; }
    }
}
