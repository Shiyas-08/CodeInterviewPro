using CodeInterviewPro.Application.Interfaces.Repositories;
using CodeInterviewPro.Application.Interfaces.Repositories.InterviewsRepositories;
using CodeInterviewPro.Application.Interfaces.Services;
using CodeInterviewPro.Domain.Entities;
using CodeInterviewPro.Domain.Enums;

namespace CodeInterviewPro.Application.Services
{
    public class InterviewSessionService : IInterviewSessionService
    {
        private readonly IInterviewSessionRepository _repository;

        public InterviewSessionService(IInterviewSessionRepository repository)
        {
            _repository = repository;
        }

        public async Task<InterviewSession> StartSessionAsync(string token)
        {
            var session = await _repository.GetByTokenAsync(token);

            if (session == null)
                throw new Exception("Session not found");

            if (session.StartTime != null)
                return session;

            session.StartTime = DateTime.UtcNow;
            session.Status = (int)InterviewSessionStatus.InProgress;

            await _repository.UpdateAsync(session);

            return session;
        }

        public async Task<InterviewSession> GetSessionAsync(string token)
        {
            var session = await _repository.GetByTokenAsync(token);

            if (session == null)
                throw new Exception("Session not found");

            return session;
        }

        public async Task StopSessionAsync(string token)
        {
            var session = await _repository.GetByTokenAsync(token);

            if (session == null)
                throw new Exception("Session not found");

            if (session.Status == (int)InterviewSessionStatus.Completed)
                throw new Exception("Interview already ended");

            session.EndTime = DateTime.UtcNow;
            session.Status = (int)InterviewSessionStatus.Completed;
            session.IsActive = false;

            await _repository.UpdateAsync(session);
        }
    }
}