using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeInterviewPro.Domain.Common.Interfaces
{
    public interface IUserContext
    {
        Guid TenantId { get; }
        long UserId { get; }
        string Role { get; }
    }
}
