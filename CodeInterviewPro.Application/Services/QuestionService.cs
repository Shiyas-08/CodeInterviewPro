using CodeInterviewPro.Application.DTOs.Interview;
using CodeInterviewPro.Application.Interfaces.Repositories.InterviewRepositories;
using CodeInterviewPro.Application.Interfaces.Services;
using CodeInterviewPro.Domain.Common.Interfaces;
using CodeInterviewPro.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeInterviewPro.Application.Services
{
    public class QuestionService : IQuestionService
    {
        private readonly IQuestionRepository _repository;
        private readonly IUserContext _userContext;

        public QuestionService(
            IQuestionRepository repository,
            IUserContext userContext)
        {
            _repository = repository;
            _userContext = userContext;
        }

        public async Task<Guid> CreateAsync(CreateQuestionDto dto)
        {
            var tenantId = _userContext.TenantId;

            var limits = GetLimits(dto.Language);

            var question = new Question
            {
                Id = Guid.NewGuid(),
                TenantId = tenantId,
                Title = dto.Title,
                Description = dto.Description,
                StarterCode = dto.StarterCode,
                TestCases = dto.TestCases,
                Language = dto.Language,
                TimeLimit = limits.time,
                MemoryLimit = limits.memory,
                CreatedAt = DateTime.UtcNow
            };

            return await _repository.CreateAsync(question);
        }

        public async Task<IEnumerable<Question>> GetAllAsync()
        {
            return await _repository.GetAllAsync(_userContext.TenantId);
        }

        public async Task<Question?> GetByIdAsync(Guid id)
        {
            return await _repository.GetByIdAsync(id, _userContext.TenantId);
        }

        public async Task UpdateAsync(UpdateQuestionDto dto)
        {
            var question = await GetByIdAsync(dto.Id);

            question.Title = dto.Title;
            question.Description = dto.Description;
            question.StarterCode = dto.StarterCode;
            question.TestCases = dto.TestCases;
            question.Language = dto.Language;

            await _repository.UpdateAsync(question);
        }

        public async Task DeleteAsync(Guid id)
        {
            await _repository.DeleteAsync(id, _userContext.TenantId);
        }

        private (int time, int memory) GetLimits(string language)
        {
            return language.ToLower() switch
            {
                "csharp" => (2, 256),
                "python" => (3, 256),
                "javascript" => (2, 256),
                "java" => (3, 512),
                _ => (2, 256)
            };
        }
    }
}
