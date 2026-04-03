using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeInterviewPro.Application.Interfaces.Services
{
    public interface IRateLimitService
    {
        Task<bool> IsAllowedAsync(
            string key,
            int limit,
            int seconds);
    }
}
