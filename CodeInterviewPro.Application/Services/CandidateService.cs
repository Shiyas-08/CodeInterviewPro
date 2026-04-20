using CodeInterviewPro.Application.DTOs;
using CodeInterviewPro.Application.Interfaces.Repositories;
using CodeInterviewPro.Application.Interfaces.Services;
using CodeInterviewPro.Domain.Common.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeInterviewPro.Application.Services
{
    public class CandidateService : ICandidateService
    {
        private readonly ICandidateRepository _repository;
        private readonly IUserContext _userContext;

        public CandidateService(
            ICandidateRepository repository,
            IUserContext userContext)
        {
            _repository = repository;
            _userContext = userContext;
        }

        public async Task<IEnumerable<CandidateInterviewDto>> GetMyInterviewsAsync()
        {
            return await _repository.GetMyInterviewsAsync(_userContext.UserId);
        }
    }
}
