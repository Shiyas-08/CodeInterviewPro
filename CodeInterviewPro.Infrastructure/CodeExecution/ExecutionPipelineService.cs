using CodeInterviewPro.Application.Interfaces.Services;
using CodeInterviewPro.Domain.Entities;
using CodeInterviewPro.Domain.Enums;
using CodeInterviewPro.Infrastructure.Cache;
using CodeInterviewPro.Infrastructure.CodeExecution.StaticAnalysis;

namespace CodeInterviewPro.Infrastructure.CodeExecution
{
    public class ExecutionPipelineService
    {
        private readonly MultiLanguageExecutionService _executionService;
        private readonly TestCaseExecutionService _testCaseService;
        private readonly IMetricsService _metricsService;
        private readonly RedisService _cache;

        public ExecutionPipelineService(
            MultiLanguageExecutionService executionService,
            TestCaseExecutionService testCaseService,
            IMetricsService metricsService,
            RedisService cache)
        {
            _executionService = executionService;
            _testCaseService = testCaseService;
            _metricsService = metricsService;
            _cache = cache;
        }

        public async Task<ExecutionResult> ExecuteAsync(
            string code,
            ProgrammingLanguage language,
            List<TestCase> testCases,
            string methodName)
        {
            // Step 1 - Static Analysis
            var analyzer =
                StaticAnalyzerFactory.GetAnalyzer(language);

            var error =
                await analyzer.AnalyzeAsync(code);

            if (!string.IsNullOrEmpty(error))
            {
                return new ExecutionResult
                {
                    Total = 0,
                    Passed = 0,
                    Failed = 0,
                    Score = 0
                };
            }

            // Step 2 - Test Execution
            var results =
                await _testCaseService.ExecuteAsync(
                    code, language, testCases, methodName);
                  

            // Step 3 - Metrics
            var metrics =
                _metricsService.Calculate(results);

            return metrics;
        }
    }
}