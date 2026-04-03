using CodeInterviewPro.Domain.Entities;
using CodeInterviewPro.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeInterviewPro.Application.Interfaces.Services
{
    public interface IAICodeReviewService
    {
        Task<AICodeReviewResult> ReviewAsync(
            string code,
            ProgrammingLanguage language);
    }
}
