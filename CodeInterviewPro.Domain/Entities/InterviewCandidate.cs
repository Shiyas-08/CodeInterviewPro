using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeInterviewPro.Domain.Entities
{
    public class InterviewCandidate
    {
        public Guid Id { get; set; }

        public Guid TenantId { get; set; }

        public Guid InterviewId { get; set; }

        public Guid CandidateId { get; set; }

        public int Status { get; set; }

        public DateTime AssignedAt { get; set; }
    }
}
