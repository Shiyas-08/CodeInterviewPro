using CodeInterviewPro.Application.Interfaces.Services;
using CodeInterviewPro.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeInterviewPro.Application.Services
{
    public class MetricsService : IMetricsService
    {
        public ExecutionResult Calculate(
            List<TestCaseResult> results)
        {
            var total = results.Count;

            var passed =
                results.Count(x => x.Passed);

            var failed =
                total - passed;

            var score =
                total == 0 ? 0 :
                (double)passed / total * 100;

            return new ExecutionResult
            {
                Total = total,
                Passed = passed,
                Failed = failed,
                Score = score
            };
        }
    }
}