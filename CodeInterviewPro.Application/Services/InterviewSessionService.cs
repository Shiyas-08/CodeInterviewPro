using Microsoft.AspNetCore.SignalR;
// Circular dependency handled by using interface-only Hub context
using CodeInterviewPro.Application.Interfaces.Services;
using CodeInterviewPro.Application.DTOs;
using CodeInterviewPro.Application.Interfaces.Repositories.InterviewRepositories;
using CodeInterviewPro.Application.Interfaces.Services;
using CodeInterviewPro.Domain.Entities;
using CodeInterviewPro.Domain.Enums;

namespace CodeInterviewPro.Application.Services
{
    public class StubHub : Hub<IInterviewHub> { }

    public class InterviewSessionService : IInterviewSessionService
    {
        private readonly IInterviewSessionRepository _sessionRepository;
        private readonly IInterviewRepository _interviewRepository;
        private readonly IQuestionRepository _questionRepository;
        private readonly IHubContext<StubHub, IInterviewHub> _hubContext;

        private readonly IInterviewInvitationRepository _invitationRepo;

        public InterviewSessionService(
            IInterviewSessionRepository sessionRepository,
            IInterviewRepository interviewRepository,
            IQuestionRepository questionRepository,
            IInterviewInvitationRepository invitationRepo,
            IHubContext<StubHub, IInterviewHub> hubContext)
        {
            _sessionRepository = sessionRepository;
            _interviewRepository = interviewRepository;
            _questionRepository = questionRepository;
            _invitationRepo = invitationRepo;
            _hubContext = hubContext;
        }

        public async Task<InterviewSession> StartSessionAsync(string token)
        {
            var session = await _sessionRepository.GetByTokenAsync(token);

            if (session == null)
                throw new Exception("Session not found");

            if (session.StartTime != null)
                return session;

            session.StartTime = DateTime.UtcNow;
            session.Status = (int)InterviewSessionStatus.InProgress;

            await _sessionRepository.UpdateAsync(session);

            return session;
        }

        public async Task<object> GetSessionAsync(string token)
        {
            var session = await _sessionRepository.GetByTokenAsync(token);

            if (session == null)
                throw new Exception("Session not found");

            var interview = await _interviewRepository.GetByIdAsync(session.InterviewId, session.TenantId);
            var invitation = await _invitationRepo.GetByTokenAsync(session.Token);

            return new
            {
                session.Id,
                session.InterviewId,
                session.CandidateId,
                session.Token,
                session.StartTime,
                session.EndTime,
                session.DurationMinutes,
                session.RemainingSeconds,
                session.Status,
                session.IsActive,
                Title = interview?.Title ?? "Coding Interview",
                CandidateEmail = invitation?.CandidateEmail ?? "Unknown"
            };
        }

        public async Task StopSessionAsync(string token)
        {
            var session = await _sessionRepository.GetByTokenAsync(token);

            if (session == null)
                throw new Exception("Session not found");

            if (session.Status == (int)InterviewSessionStatus.Completed)
                throw new Exception("Interview already ended");

            session.EndTime = DateTime.UtcNow;
            session.Status = (int)InterviewSessionStatus.Completed;
            session.IsActive = false;

            await _sessionRepository.UpdateAsync(session);

            // Also update parent interview status
            var interview = await _interviewRepository.GetByIdAsync(session.InterviewId, session.TenantId);
            if (interview != null)
            {
                interview.Status = (int)InterviewStatus.Completed;
                await _interviewRepository.UpdateAsync(interview);
            }

            // Notify candidate in real-time
            await _hubContext.Clients.Group($"Candidate_{token}").InterviewStopped();
        }

        public async Task<ResumeInterviewResponse> ResumeSessionAsync(string token)
        {
            var session = await _sessionRepository.GetByTokenAsync(token);

            if (session == null)
                throw new Exception("Session not found");

            var interview = await _interviewRepository.GetByIdAsync(
                session.InterviewId,
                session.TenantId);

            var questions = await _questionRepository
                .GetByInterviewIdAsync(session.InterviewId);

            return new ResumeInterviewResponse
            {
                SessionId = session.Id,
                InterviewId = session.InterviewId,
                Title = interview?.Title ?? "Coding Interview",

                DurationMinutes = session.DurationMinutes,
                RemainingSeconds = session.RemainingSeconds,
                Status = session.Status,

                Questions = questions.ToList()
            };
        }
    }
}