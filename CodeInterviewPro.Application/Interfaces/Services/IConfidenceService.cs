using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace CodeInterviewPro.Application.Interfaces.Services
{
    public interface IConfidenceService
    {
        double CalculateConfidence(
            double aiScore,
            double deepScore,
            double codeBertScore);
    }
}