using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeInterviewPro.Application.Interfaces.Services
{
    public interface IExecutionCacheService
    {
        Task<string?> GetAsync(string key);
        Task SetAsync(string key, string value);
    }
}
