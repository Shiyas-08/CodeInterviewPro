using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeInterviewPro.Domain.Entities
{
    public class InterviewQuestion
    {
        public long Id { get; set; }

        public long TenantId { get; set; }

        public long InterviewId { get; set; }

        public long QuestionId { get; set; }

        public int Marks { get; set; }
    }
}
