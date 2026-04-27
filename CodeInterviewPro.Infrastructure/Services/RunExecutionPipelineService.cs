// ============================================
// Infrastructure/Services
// RunExecutionPipelineService.cs
// LIGHT PIPELINE - NO AI / NO GEMINI
// ============================================

using CodeInterviewPro.Application.Common.Execution;
using CodeInterviewPro.Application.Interfaces.Services;
using CodeInterviewPro.Domain.Entities;
using CodeInterviewPro.Domain.Enums;
using CodeInterviewPro.Infrastructure.CodeExecution;

namespace CodeInterviewPro.Infrastructure.Services
{
    public class RunExecutionPipelineService
        : IRunExecutionPipelineService
    {
        private readonly TestCaseExecutionService _testCaseService;
        private readonly IMetricsService _metricsService;

        public RunExecutionPipelineService(
            TestCaseExecutionService testCaseService,
            IMetricsService metricsService)
        {
            _testCaseService = testCaseService;
            _metricsService = metricsService;
        }

        public async Task<ExecutionResult> ExecuteAsync(
            string code,
            ProgrammingLanguage language,
            List<TestCase> testCases,
            string methodName)
        {
            try
            {
                var analyzer = CodeInterviewPro.Infrastructure.CodeExecution.StaticAnalysis.StaticAnalyzerFactory.GetAnalyzer(language);
                var compilerErrors = await analyzer.AnalyzeAsync(code);

                if (!string.IsNullOrWhiteSpace(compilerErrors))
                {
                    return new ExecutionResult
                    {
                        Total = 0, Passed = 0, Failed = 0, Score = 0, FinalScore = 0,
                        AIFeedback = compilerErrors
                    };
                }

                var results =
                    await _testCaseService.ExecuteAsync(
                        code,
                        language,
                        testCases,
                        methodName,
                        CancellationToken.None);

                var metrics =
                    _metricsService.Calculate(results);

                return new ExecutionResult
                {
                    Total = metrics.Total,
                    Passed = metrics.Passed,
                    Failed = metrics.Failed,
                    Score = metrics.Score,
                    FinalScore = metrics.Score,

                    AIFeedback = "",
                    AIScore = 0,
                    AIComplexity = "",
                    Similarity = 0,
                    SimilarityMessage = ""
                };
            }
            catch (Exception ex)
            {
                return new ExecutionResult
                {
                    Total = 0,
                    Passed = 0,
                    Failed = 0,
                    Score = 0,
                    FinalScore = 0,
                    AIFeedback = ex.Message
                };
            }
        }
    }
}