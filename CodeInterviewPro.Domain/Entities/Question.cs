using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeInterviewPro.Domain.Entities
{
    public class Question
    {
        public Guid Id { get; set; }

        public Guid TenantId { get; set; }

        public string Title { get; set; } = default!;

        public string Description { get; set; } = default!;

        public string? StarterCode { get; set; }

        public string TestCases { get; set; } = default!;

        public int TimeLimit { get; set; }

        public int MemoryLimit { get; set; }

        public string Language { get; set; } = default!;

        public bool IsActive { get; set; } = true;

        public DateTime CreatedAt { get; set; }
    }
}
