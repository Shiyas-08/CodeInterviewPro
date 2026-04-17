using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeInterviewPro.Application.Interfaces.Services
{
    public interface IResultPdfService
    {
        /// <summary>
        /// Generate interview result PDF for a candidate
        /// </summary>
        /// <param name="candidateId">Candidate User Id</param>
        /// <returns>PDF file as byte array</returns>
        Task<byte[]> GenerateAsync(Guid candidateId);
    }
}