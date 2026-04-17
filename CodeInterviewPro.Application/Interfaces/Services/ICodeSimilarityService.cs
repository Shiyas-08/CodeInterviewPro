using CodeInterviewPro.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace CodeInterviewPro.Application.Interfaces.Services
{
    public interface ICodeSimilarityService
    {
        Task<CodeSimilarityResult> CheckSimilarityAsync(
            string code,
            string language);
    }
    }           