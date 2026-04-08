using CodeInterviewPro.Domain.Common;
using CodeInterviewPro.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeInterviewPro.Domain.Entities
{
    public class User:BaseEntity
    {
        public Guid Id { get; set; }

        public Guid? TenantId { get; set; }

        public string Email { get; set; }

        public string PasswordHash { get; set; }

        public string FullName { get; set; }

        public UserRole Role { get; set; }
    }
}
