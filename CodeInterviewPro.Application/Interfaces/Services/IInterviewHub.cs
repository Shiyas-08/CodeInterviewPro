using System.Threading.Tasks;

namespace CodeInterviewPro.Application.Interfaces.Services
{
    public interface IInterviewHub
    {
        Task CodeChanged(string sessionId, string code);
        Task CandidateStatusChanged(bool isOnline);
        Task TimerUpdated(int remainingSeconds);
        Task InterviewSubmitted();
        Task InterviewStopped();
        Task ReceiveWebRTCOffer(string offer);
        Task ReceiveWebRTCAnswer(string answer);
        Task ReceiveIceCandidate(string candidate);
        Task ReceiveCode(string code);
        Task PingCandidate();
    }
}
