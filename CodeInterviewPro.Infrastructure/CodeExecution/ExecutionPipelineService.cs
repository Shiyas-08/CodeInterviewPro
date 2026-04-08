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
        private readonly ICodeSimilarityService _similarity;
        private readonly IScoringService _scoring;

        public ExecutionPipelineService(
            MultiLanguageExecutionService executionService,
            TestCaseExecutionService testCaseService,
            IMetricsService metricsService,
            IExecutionCacheService cache,
            IExecutionHistoryRepository repository,
            IRateLimitService rateLimit,
            IExecutionTimeoutService timeout,
            IExecutionResourceService resource,
            IAICodeReviewService aiReview,
            ICodeSimilarityService similarity,
            IScoringService scoring)
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
            _similarity = similarity;
            _scoring = scoring;
        }

        public async Task<ExecutionResult> ExecuteAsync(
            string code,
            ProgrammingLanguage language,
            List<TestCase> testCases,
            string methodName)
        {
            Console.WriteLine("========== PIPELINE START ==========");
            Console.WriteLine($"Language: {language}");
            Console.WriteLine($"Method: {methodName}");

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
                  System.Text.Json.JsonSerializer.Serialize(testCases));

            // Step 3 - Cache
            var cached =
                await _cache.GetAsync(cacheKey);

            if (!string.IsNullOrEmpty(cached))
            {
                Console.WriteLine("CACHE HIT");
                return System.Text.Json.JsonSerializer
                    .Deserialize<ExecutionResult>(cached);
            }

            Console.WriteLine("CACHE MISS");

            // Step 4 - Static Analysis
            Console.WriteLine("STATIC ANALYSIS START");

            var analyzer =
                StaticAnalyzerFactory.GetAnalyzer(language);

            var error =
                await analyzer.AnalyzeAsync(code);

            Console.WriteLine("STATIC ANALYSIS RESULT:");
            Console.WriteLine(error);

            if (!string.IsNullOrEmpty(error))
            {
                Console.WriteLine("STATIC ANALYSIS FAILED");

                return new ExecutionResult
                {
                    Total = 0,
                    Passed = 0,
                    Failed = 0,
                    Score = 0
                };
            }

            Console.WriteLine("STATIC ANALYSIS PASSED");

            // Step 5 - Test Execution
            Console.WriteLine("TEST EXECUTION START");
            var results =
         await _resource.ExecuteWithLimits(
             () => _timeout.ExecuteWithTimeout<List<TestCaseResult>>(
                 token => _testCaseService.ExecuteAsync(
                     code,
                     language,
                     testCases,
                     methodName,
                     token),
                 30),
             256,
             1);

            Console.WriteLine("TEST EXECUTION COMPLETED");

            // Step 6 - Metrics
            var metrics =
                _metricsService.Calculate(results);

            Console.WriteLine($"TOTAL: {metrics.Total}");
            Console.WriteLine($"PASSED: {metrics.Passed}");
            Console.WriteLine($"FAILED: {metrics.Failed}");

            // Step 7 - AI Review
            var aiReview =
                await _aiReview.ReviewAsync(
                    code,
                    language);

            metrics.AIScore = aiReview.Score;
            metrics.AIFeedback = aiReview.Feedback;
            metrics.AIComplexity = aiReview.Complexity;

            var similarity =
                await _similarity.CheckSimilarityAsync(
                    code,
                    language.ToString());

            metrics.Similarity =
                similarity.SimilarityPercentage;

            metrics.SimilarityMessage =
                similarity.Message;

            metrics.FinalScore =
                _scoring.Calculate(
                    metrics.Score,
                    metrics.AIScore,
                    metrics.Similarity);

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
                FinalScore = metrics.FinalScore,
                CreatedAt = DateTime.UtcNow
            };

            await _repository.SaveAsync(history);

            // Step 9 - Cache
            await _cache.SetAsync(
                cacheKey,
                System.Text.Json.JsonSerializer.Serialize(metrics));

            Console.WriteLine("========== PIPELINE END ==========");

            return metrics;
        }
    }
}