using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeInterviewPro.Application.Interfaces.Services
{
    public interface IScoringService
    {
        double Calculate(
            double executionScore,
            double aiScore,
            double similarity);
    }
}