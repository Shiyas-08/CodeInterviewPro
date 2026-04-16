using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeInterviewPro.Application.DTOs.Interview
{
    public class InviteCandidateDto
    {
        public string Email { get; set; }
        public string Name { get; set; }
        public DateTime ExpiryTime { get; set; }
    }
}
