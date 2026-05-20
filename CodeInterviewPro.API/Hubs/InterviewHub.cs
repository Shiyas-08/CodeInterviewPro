using CodeInterviewPro.Application.Interfaces.Repositories.InterviewRepositories;
using CodeInterviewPro.Application.Interfaces.Services;
using CodeInterviewPro.Infrastructure.Cache;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;
using System;

namespace CodeInterviewPro.API.Hubs
{
    [Authorize]
    public class InterviewHub : Hub<IInterviewHub>
    {
        private readonly ICacheService _redis;
        private readonly IInterviewSessionRepository _sessionRepo;

        public InterviewHub(ICacheService redis, IInterviewSessionRepository sessionRepo)
        {
            _redis = redis;
            _sessionRepo = sessionRepo;
        }

        public async Task JoinCandidateRoom(string sessionId)
        {
            sessionId = sessionId.ToLowerInvariant();
            
            // SECURITY: Validate candidate owns this session
            var session = await _sessionRepo.GetByTokenAsync(sessionId);
            var userId = Context.User?.FindFirst("uid")?.Value;
            
            if (session == null || session.CandidateId.ToString() != userId)
            {
                throw new HubException("Unauthorized to join this room.");
            }

            await Groups.AddToGroupAsync(Context.ConnectionId, $"Candidate_{sessionId}");
            // Broadcast candidate is online to HR
            await Clients.Group($"HR_{sessionId}").CandidateStatusChanged(true);
        }

        public async Task JoinHRRoom(string sessionId)
        {
            sessionId = sessionId.ToLowerInvariant();
            
            // SECURITY: Validate HR owns this session (belongs to their tenant)
            var session = await _sessionRepo.GetByTokenAsync(sessionId);
            var tenantId = Context.User?.FindFirst("tid")?.Value;
            var roleId = Context.User?.FindFirst("rid")?.Value;
            
            if (session == null || (roleId == "2" && session.TenantId.ToString() != tenantId))
            {
                throw new HubException("Unauthorized to monitor this room.");
            }

            await Groups.AddToGroupAsync(Context.ConnectionId, $"HR_{sessionId}");
            
            // Send latest code from redis if available
            var code = await _redis.GetAsync<string>($"code:{sessionId}");
            if (!string.IsNullOrEmpty(code))
            {
                await Clients.Caller.CodeChanged(sessionId, code);
            }

            // Ping candidate to confirm online status
            await Clients.Group($"Candidate_{sessionId}").PingCandidate();
        }

        public async Task CodeChanged(string sessionId, string code)
        {
            try
            {
                sessionId = sessionId.ToLowerInvariant();
                System.Console.WriteLine($"[Hub] CodeChanged for {sessionId}, code length: {code?.Length ?? 0}");

                // Store latest code in Redis
                await _redis.SetAsync($"code:{sessionId}", code);

                // Broadcast code to HR room
                await Clients.Group($"HR_{sessionId}").CodeChanged(sessionId, code);
            }
            catch (System.Exception ex)
            {
                System.Console.WriteLine($"[Hub Error] CodeChanged failed: {ex.Message}");
                // Still try to broadcast even if Redis fails
                await Clients.Group($"HR_{sessionId.ToLowerInvariant()}").CodeChanged(sessionId, code);
            }
        }

        public async Task CandidateStatusChanged(string sessionId, bool isOnline)
        {
            sessionId = sessionId.ToLowerInvariant();
            System.Console.WriteLine($"[Hub] CandidateStatusChanged: {sessionId} is {isOnline}");
            await Clients.Group($"HR_{sessionId}").CandidateStatusChanged(isOnline);
        }

        public async Task TimerUpdated(string sessionId, int remainingSeconds)
        {
            sessionId = sessionId.ToLowerInvariant();
            await Clients.Group($"HR_{sessionId}").TimerUpdated(remainingSeconds);
        }

        public async Task InterviewSubmitted(string sessionId)
        {
            sessionId = sessionId.ToLowerInvariant();
            await Clients.Group($"HR_{sessionId}").InterviewSubmitted();
        }

        public async Task InterviewStopped(string sessionId)
        {
            sessionId = sessionId.ToLowerInvariant();
            await Clients.Group($"Candidate_{sessionId}").InterviewStopped();
        }

        public async Task PingCandidate(string sessionId)
        {
            sessionId = sessionId.ToLowerInvariant();
            await Clients.Group($"Candidate_{sessionId}").PingCandidate();
        }

        // ==========================
        // WEBRTC SIGNALING (PHASE 2)
        // ==========================

        public async Task SendWebRTCOffer(string sessionId, string offer, bool isFromHR)
        {
            sessionId = sessionId.ToLowerInvariant();
            string targetGroup = isFromHR ? $"Candidate_{sessionId}" : $"HR_{sessionId}";
            await Clients.Group(targetGroup).ReceiveWebRTCOffer(offer);
        }

        public async Task SendWebRTCAnswer(string sessionId, string answer, bool isFromHR)
        {
            sessionId = sessionId.ToLowerInvariant();
            string targetGroup = isFromHR ? $"Candidate_{sessionId}" : $"HR_{sessionId}";
            await Clients.Group(targetGroup).ReceiveWebRTCAnswer(answer);
        }

        public async Task SendIceCandidate(string sessionId, string candidate, bool isFromHR)
        {
            sessionId = sessionId.ToLowerInvariant();
            string targetGroup = isFromHR ? $"Candidate_{sessionId}" : $"HR_{sessionId}";
            await Clients.Group(targetGroup).ReceiveIceCandidate(candidate);
        }

        // End of WebRTC Signaling
    }
}