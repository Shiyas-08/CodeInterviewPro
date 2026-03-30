using Microsoft.AspNetCore.SignalR;

namespace CodeInterviewPro.API.Hubs
{
    public class InterviewHub : Hub
    {
        public async Task JoinInterview(string interviewId)
        {
            Console.WriteLine($"User joined: {interviewId}");

            await Groups.AddToGroupAsync(
                Context.ConnectionId,
                interviewId);
        }

        public async Task SendCode(string interviewId, string code)
        {
            Console.WriteLine($"Code received: {code}");

            await Clients
                .Group(interviewId)
                .SendAsync("ReceiveCode", code);
        }
    }
}