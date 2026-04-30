using CodeInterviewPro.Application.Interfaces.Services;
using CodeInterviewPro.API.Hubs;
using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;

namespace CodeInterviewPro.API.Services
{
    public class InterviewNotificationService : CodeInterviewPro.Application.Interfaces.Services.IInterviewNotificationService
    {
        private readonly IHubContext<InterviewHub, IInterviewHub> _hubContext;

        public InterviewNotificationService(IHubContext<InterviewHub, IInterviewHub> hubContext)
        {
            _hubContext = hubContext;
        }

        public async Task NotifyInterviewStoppedAsync(string sessionId, string token)
        {
            // Notify via Session ID group
            if (!string.IsNullOrEmpty(sessionId))
            {
                await _hubContext.Clients.Group($"Candidate_{sessionId.ToLowerInvariant()}").InterviewStopped();
            }

            // Notify via Token group (fallback)
            if (!string.IsNullOrEmpty(token))
            {
                await _hubContext.Clients.Group($"Candidate_{token.ToLowerInvariant()}").InterviewStopped();
            }
        }
    }
}
