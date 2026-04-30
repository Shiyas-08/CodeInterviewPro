using System.Threading.Tasks;

namespace CodeInterviewPro.Application.Interfaces.Services
{
    public interface IInterviewNotificationService
    {
        Task NotifyInterviewStoppedAsync(string sessionId, string token);
    }
}
