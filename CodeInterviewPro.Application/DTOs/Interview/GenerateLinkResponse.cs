using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeInterviewPro.Application.DTOs.Interview
{
    public class GenerateLinkResponse
    {
        public string Token { get; set; } = string.Empty;

        public string Link { get; set; } = string.Empty;

        public DateTime ExpiryTime { get; set; }
    }
}
