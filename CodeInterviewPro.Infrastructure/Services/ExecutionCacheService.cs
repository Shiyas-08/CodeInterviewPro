using CodeInterviewPro.Application.Interfaces.Services;
using CodeInterviewPro.Infrastructure.Cache;
using System;
using System.Threading.Tasks;

namespace CodeInterviewPro.Infrastructure.Services
{
    public class ExecutionCacheService : IExecutionCacheService
    {
        private readonly ICacheService _redis;

        public ExecutionCacheService(ICacheService redis)
        {
            _redis = redis;
        }

        public async Task<string?> GetAsync(string key)
        {
            return await _redis.GetAsync<string>(key);
        }

        public async Task SetAsync(string key, string value)
        {
            await _redis.SetAsync(
                key,
                value,
                TimeSpan.FromMinutes(10));
        }
    }
}