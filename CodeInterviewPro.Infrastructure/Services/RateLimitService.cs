using CodeInterviewPro.Application.Interfaces.Services;
using CodeInterviewPro.Infrastructure.Cache;

namespace CodeInterviewPro.Infrastructure.Services
{
    public class RateLimitService : IRateLimitService
    {
        private readonly ICacheService _redis;

        public RateLimitService(ICacheService redis)
        {
            _redis = redis;
        }

        public async Task<bool> IsAllowedAsync(
            string key,
            int limit,
            int seconds)
        {
            var count =
                await _redis.GetAsync<int>(key);

            if (count >= limit)
                return false;

            count++;

            await _redis.SetAsync(
                key,
                count,
                TimeSpan.FromSeconds(seconds));

            return true;
        }
    }
}