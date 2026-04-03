using CodeInterviewPro.Application.Common.Execution;
using CodeInterviewPro.Application.Interfaces.Repositories;
using CodeInterviewPro.Application.Interfaces.Services;
using CodeInterviewPro.Domain.Entities;
using CodeInterviewPro.Domain.Enums;
using CodeInterviewPro.Infrastructure.CodeExecution.StaticAnalysis;

namespace CodeInterviewPro.Infrastructure.CodeExecution
{
    public class ExecutionPipelineService
    {
        private readonly MultiLanguageExecutionService _executionService;
        private readonly TestCaseExecutionService _testCaseService;
        private readonly IMetricsService _metricsService;
        private readonly IExecutionCacheService _cache;
        private readonly IExecutionHistoryRepository _repository;
        private readonly IRateLimitService _rateLimit;
        private readonly IExecutionTimeoutService _timeout;
        private readonly IExecutionResourceService _resource;
        private readonly IAICodeReviewService _aiReview;

        public ExecutionPipelineService(
            MultiLanguageExecutionService executionService,
            TestCaseExecutionService testCaseService,
            IMetricsService metricsService,
            IExecutionCacheService cache,
            IExecutionHistoryRepository repository,
            IRateLimitService rateLimit,
            IExecutionTimeoutService timeout,
            IExecutionResourceService resource,
            IAICodeReviewService aiReview)
        {
            _executionService = executionService;
            _testCaseService = testCaseService;
            _metricsService = metricsService;
            _cache = cache;
            _repository = repository;
            _rateLimit = rateLimit;
            _timeout = timeout;
            _resource = resource;
            _aiReview = aiReview;
        }

        public async Task<ExecutionResult> ExecuteAsync(
            string code,
            ProgrammingLanguage language,
            List<TestCase> testCases,
            string methodName)
        {
            // Step 1 - Rate Limit
            var rateKey =
                $"execution_rate:{methodName}";

            var allowed =
                await _rateLimit.IsAllowedAsync(
                    rateKey,
                    5,
                    60);

            if (!allowed)
            {
                throw new Exception(
                    "Too many executions. Try again later.");
            }

            // Step 2 - Cache Key
            var cacheKey =
                ExecutionCacheKeyGenerator.Generate(
                    code,
                    language.ToString(),
                    methodName);

            // Step 3 - Check Cache
            var cached =
                await _cache.GetAsync(cacheKey);

            if (!string.IsNullOrEmpty(cached))
            {
                return System.Text.Json.JsonSerializer
                    .Deserialize<ExecutionResult>(cached);
            }

            // Step 4 - Static Analysis
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

            // Step 5 - Test Execution (Timeout + Resource Limit)
            var results =
                await _resource.ExecuteWithLimits(
                    () => _timeout.ExecuteWithTimeout(
                        () => _testCaseService.ExecuteAsync(
                            code,
                            language,
                            testCases,
                            methodName),
                        10),
                    256,
                    1);

            // Step 6 - Metrics
            var metrics =
                _metricsService.Calculate(results);

            // Step 7 - AI Review (NEW)
            var aiReview =
                await _aiReview.ReviewAsync(
                    code,
                    language);

            metrics.AIScore = aiReview.Score;
            metrics.AIFeedback = aiReview.Feedback;
            metrics.AIComplexity = aiReview.Complexity;

            // Step 8 - Save History
            var history = new ExecutionHistory
            {
                Id = Guid.NewGuid(),
                Code = code,
                Language = language.ToString(),
                Total = metrics.Total,
                Passed = metrics.Passed,
                Failed = metrics.Failed,
                Score = (int)metrics.Score,
                AIScore = metrics.AIScore,
                AIFeedback = metrics.AIFeedback,
                AIComplexity = metrics.AIComplexity,
                CreatedAt = DateTime.UtcNow
            };

            await _repository.SaveAsync(history);

            // Step 9 - Cache Result
            await _cache.SetAsync(
                cacheKey,
                System.Text.Json.JsonSerializer.Serialize(metrics));

            return metrics;
        }
    }
}