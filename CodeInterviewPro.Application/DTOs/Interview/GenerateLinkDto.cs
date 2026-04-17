using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeInterviewPro.Application.DTOs.Interview
{
    public class GenerateLinkDto
    {
        public Guid CandidateId { get; set; }

        public DateTime ExpiryTime { get; set; }
    }
}
