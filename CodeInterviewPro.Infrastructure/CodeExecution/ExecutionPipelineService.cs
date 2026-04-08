using CodeInterviewPro.Application.AI;
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
        private readonly ICodeSimilarityService _similarity;
        private readonly IScoringService _scoring;
        private readonly AIIntelligenceService _aiIntelligence;

        public ExecutionPipelineService(
            MultiLanguageExecutionService executionService,
            TestCaseExecutionService testCaseService,
            IMetricsService metricsService,
            IExecutionCacheService cache,
            IExecutionHistoryRepository repository,
            IRateLimitService rateLimit,
            IExecutionTimeoutService timeout,
            IExecutionResourceService resource,
            ICodeSimilarityService similarity,
            IScoringService scoring,
            AIIntelligenceService aiIntelligence)
        {
            _executionService = executionService;
            _testCaseService = testCaseService;
            _metricsService = metricsService;
            _cache = cache;
            _repository = repository;
            _rateLimit = rateLimit;
            _timeout = timeout;
            _resource = resource;
            _similarity = similarity;
            _scoring = scoring;
            _aiIntelligence = aiIntelligence;
        }

        public async Task<ExecutionResult> ExecuteAsync(
            string code,
            ProgrammingLanguage language,
            List<TestCase> testCases,
            string methodName)
        {
            Console.WriteLine("========== PIPELINE START ==========");

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

            // Step 7 - AI Intelligence
            Console.WriteLine("AI INTELLIGENCE START");

            var aiResult =
                _aiIntelligence.Analyze(code);

            metrics.AIScore =
                aiResult.FinalScore;

            metrics.AIFeedback =
                aiResult.Feedback;

            metrics.AIComplexity =
                aiResult.Semantic.Complexity;

            // Step 8 - Similarity
            var similarity =
                await _similarity.CheckSimilarityAsync(
                    code,
                    language.ToString());

            metrics.Similarity =
                similarity.SimilarityPercentage;

            metrics.SimilarityMessage =
                similarity.Message;

            // Step 9 - Final Score
            metrics.FinalScore =
                _scoring.Calculate(
                    metrics.Score,
                    metrics.AIScore,
                    metrics.Similarity);

            // Step 10 - Save History
            var history =
                new ExecutionHistory
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

            // Step 11 - Cache
            await _cache.SetAsync(
                cacheKey,
                System.Text.Json.JsonSerializer.Serialize(metrics));

            Console.WriteLine("========== PIPELINE END ==========");

            return metrics;
        }
    }
}