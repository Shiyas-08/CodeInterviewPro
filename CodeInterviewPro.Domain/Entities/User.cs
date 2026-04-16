using CodeInterviewPro.Domain.Common;
using CodeInterviewPro.Domain.Enums;

namespace CodeInterviewPro.Domain.Entities
{
    public class User : BaseEntity
    {
        public Guid Id { get; set; }

        public Guid? TenantId { get; set; }

        public string Email { get; set; }

        public string PasswordHash { get; set; }

        public string FullName { get; set; }

        public UserRole Role { get; set; }

        public bool IsActive { get; set; }
    }
}