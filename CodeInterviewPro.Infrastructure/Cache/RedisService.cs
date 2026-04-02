using StackExchange.Redis;
using System.Text.Json;

namespace CodeInterviewPro.Infrastructure.Cache
{
    public class RedisService
    {
        private readonly IDatabase _database;

        public RedisService(IConnectionMultiplexer redis)
        {
            _database = redis.GetDatabase();
        }

        public async Task SetAsync<T>(string key, T value)
        {
            var json = JsonSerializer.Serialize(value);

            await _database.StringSetAsync(key, json);
        }

        public async Task<T> GetAsync<T>(string key)
        {
            var value =
                await _database.StringGetAsync(key);

            if (value.IsNullOrEmpty)
                return default;

            return JsonSerializer.Deserialize<T>(value);
        }
    }
}