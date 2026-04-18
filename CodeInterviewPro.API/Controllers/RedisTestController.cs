using CodeInterviewPro.Application.Interfaces.Services;
using CodeInterviewPro.Infrastructure.Cache;
using Microsoft.AspNetCore.Mvc;

namespace CodeInterviewPro.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RedisTestController : ControllerBase
    {
        private readonly ICacheService _redis;

        public RedisTestController(ICacheService redis)
        {
            _redis = redis;
        }

        [HttpGet]
        public async Task<IActionResult> Test()
        {
            await _redis.SetAsync("test", "Hello Redis");

            var value = await _redis.GetAsync<string>("test");

            return Ok(value);
        }
    }
}