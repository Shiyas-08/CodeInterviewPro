using CodeInterviewPro.Application.Interfaces.Repositories;
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
        private readonly IExecutionHistoryRepository _repository;

        public ExecutionPipelineService(
            MultiLanguageExecutionService executionService,
            TestCaseExecutionService testCaseService,
            IMetricsService metricsService,
            RedisService cache,
            IExecutionHistoryRepository repository)
        {
            _executionService = executionService;
            _testCaseService = testCaseService;
            _metricsService = metricsService;
            _cache = cache;
            _repository = repository;
        }

        public async Task<ExecutionResult> ExecuteAsync(
    string code,
    ProgrammingLanguage language,
    List<TestCase> testCases,
    string methodName)
        {
            var cacheKey =
                $"execution:{language}:{code.GetHashCode()}";

            // Step 1 - Check Cache
            var cached =
                await _cache.GetAsync<ExecutionResult>(cacheKey);

            if (cached != null)
                return cached;

            // Step 2 - Static Analysis
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

            // Step 3 - Test Execution
            var results =
                await _testCaseService.ExecuteAsync(
                    code,
                    language,
                    testCases,
                    methodName);

            // Step 4 - Metrics
            var metrics =
                _metricsService.Calculate(results);

            // Step 5 - Save History
            var history = new ExecutionHistory
            {
                Id = Guid.NewGuid(),
                Code = code,
                Language = language.ToString(),
                Total = metrics.Total,
                Passed = metrics.Passed,
                Failed = metrics.Failed,
                Score = metrics.Score,
                CreatedAt = DateTime.UtcNow
            };

            await _repository.SaveAsync(history);

            // Step 6 - Cache Result
            await _cache.SetAsync(cacheKey, metrics);

            return metrics;
        }
    }
}