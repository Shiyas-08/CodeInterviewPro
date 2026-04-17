using CodeInterviewPro.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeInterviewPro.Application.Interfaces.Services
{
    public interface ICodeBertService
    {
        Task<CodeBertResult> AnalyzeAsync(string code, string language);

    }
}
