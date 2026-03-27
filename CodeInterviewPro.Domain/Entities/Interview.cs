using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeInterviewPro.Domain.Entities
{
    public class Interview
    {
        public long Id { get; set; }

        public Guid TenantId { get; set; }

        public string Title { get; set; } = string.Empty;

        public string? Description { get; set; }

        public int DurationMinutes { get; set; }

        public DateTime? StartTime { get; set; }

        public DateTime? EndTime { get; set; }

        public int Status { get; set; }

        public long CreatedBy { get; set; }

        public DateTime CreatedAt { get; set; }

        public bool IsActive { get; set; }
    }
}
