using Microsoft.AspNetCore.SignalR;
using CodeInterviewPro.Infrastructure.Cache;

namespace CodeInterviewPro.API.Hubs
{
    public class InterviewHub : Hub
    {
        private readonly RedisService _redis;

        public InterviewHub(RedisService redis)
        {
            _redis = redis;
        }

        public async Task JoinInterview(string interviewId)
        {
            Console.WriteLine($"User joined: {interviewId}");

            await Groups.AddToGroupAsync(
                Context.ConnectionId,
                interviewId);

            // Load previous code from Redis
            var code = await _redis.GetAsync($"code:{interviewId}");

            if (!string.IsNullOrEmpty(code))
            {
                await Clients.Caller.SendAsync("ReceiveCode", code);
            }
        }

        public async Task SendCode(string interviewId, string code)
        {
            Console.WriteLine($"Code received: {code}");

            // Store latest code in Redis
            await _redis.SetAsync($"code:{interviewId}", code);

            // Broadcast code to all users
            await Clients
                .Group(interviewId)
                .SendAsync("ReceiveCode", code);
        }
    }
}