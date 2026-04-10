using CodeInterviewPro.Application.AI;
using CodeInterviewPro.Application.Common.Execution;
using CodeInterviewPro.Application.Interfaces.Repositories;
using CodeInterviewPro.Application.Interfaces.Services;
using CodeInterviewPro.Domain.Entities;
using CodeInterviewPro.Domain.Enums;
using CodeInterviewPro.Infrastructure.CodeExecution.StaticAnalysis;
using CodeInterviewPro.Infrastructure.CodeExecution.StaticAnalysis.Roslyn;
using CodeInterviewPro.Infrastructure.StaticAnalysis.ESLint;
using CodeInterviewPro.Infrastructure.StaticAnalysis.Go;
using CodeInterviewPro.Infrastructure.StaticAnalysis.Java;
using CodeInterviewPro.Infrastructure.StaticAnalysis.Python;

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
        private readonly IDeepAnalysisService _deepAnalysis;
        private readonly ICodeBertService _codeBert;
        private readonly IConfidenceService _confidence;

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
            AIIntelligenceService aiIntelligence,
            IDeepAnalysisService deepAnalysis,
            ICodeBertService codeBert,
            IConfidenceService confidence)
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
            _deepAnalysis = deepAnalysis;
            _codeBert = codeBert;
            _confidence = confidence;
        }

        public async Task<ExecutionResult> ExecuteAsync(
            string code,
            ProgrammingLanguage language,
            List<TestCase> testCases,
            string methodName)
        {
            Console.WriteLine("========== PIPELINE START ==========");

            // Step 1 - Rate Limit

            var rateKey = $"execution_rate:{methodName}";

            var allowed = await _rateLimit.IsAllowedAsync(
                rateKey,
                5,
                60);

            if (!allowed)
            {
                throw new Exception(
                    "Too many executions. Try again later.");
            }

            // Step 2 - Cache

            var cacheKey =
                ExecutionCacheKeyGenerator.Generate(
                    code,
                    language.ToString(),
                    System.Text.Json.JsonSerializer.Serialize(testCases));

            var cached = await _cache.GetAsync(cacheKey);

            if (!string.IsNullOrEmpty(cached))
            {
                Console.WriteLine("CACHE HIT");

                return System.Text.Json.JsonSerializer
                    .Deserialize<ExecutionResult>(cached);
            }

            Console.WriteLine("CACHE MISS");

            // Step 3 - Static Analysis

            Console.WriteLine("STATIC ANALYSIS START");

            if (language == ProgrammingLanguage.CSharp)
            {
                var roslyn = new RoslynAnalyzer();
                var errors = roslyn.Analyze(code);

                if (errors.Any())
                {
                    return new ExecutionResult
                    {
                        AIFeedback = string.Join("\n", errors)
                    };
                }
            }

            if (language == ProgrammingLanguage.JavaScript)
            {
                var eslint = new ESLintAnalyzer();
                var errors = await eslint.AnalyzeAsync(code);

                if (errors.Any())
                {
                    return new ExecutionResult
                    {
                        AIFeedback = string.Join("\n", errors)
                    };
                }
            }

            if (language == ProgrammingLanguage.Python)
            {
                var pylint = new PylintAnalyzer();
                var errors = await pylint.AnalyzeAsync(code);

                if (errors.Any())
                {
                    return new ExecutionResult
                    {
                        AIFeedback = string.Join("\n", errors)
                    };
                }
            }

            if (language == ProgrammingLanguage.Go)
            {
                var go = new GoVetAnalyzer();
                var errors = await go.AnalyzeAsync(code);

                if (errors.Any())
                {
                    return new ExecutionResult
                    {
                        AIFeedback = string.Join("\n", errors)
                    };
                }
            }

            if (language == ProgrammingLanguage.Java)
            {
                var java = new JavaAnalyzer();
                var errors = await java.AnalyzeAsync(code);

                if (errors.Any())
                {
                    return new ExecutionResult
                    {
                        AIFeedback = string.Join("\n", errors)
                    };
                }
            }

            Console.WriteLine("STATIC ANALYSIS PASSED");

            // Step 4 - Execution

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

            // Step 5 - Metrics

            var metrics = _metricsService.Calculate(results);

            // Step 6 - AI Intelligence

            var aiResult = _aiIntelligence.Analyze(code);

            // Step 6.5 - Deep Analysis

            var deepResult =
                await _deepAnalysis.AnalyzeAsync(
                    code,
                    language.ToString());

            // Step 6.7 - CodeBERT

            var codeBertResult =
                await _codeBert.AnalyzeAsync(
                    code,
                    language.ToString());

            // Step 6.9 - Confidence Engine

            var confidenceScore =
                _confidence.CalculateConfidence(
                    aiResult.FinalScore,
                    deepResult.Score,
                    codeBertResult.Score);

            metrics.AIScore = confidenceScore;

            metrics.AIFeedback =
                aiResult.Feedback +
                "\nDeep Analysis: " + deepResult.Feedback +
                "\nCodeBERT: " + codeBertResult.Feedback;

            metrics.AIComplexity =
                codeBertResult.Complexity;

            // Step 7 - Similarity

            var similarity =
                await _similarity.CheckSimilarityAsync(
                    code,
                    language.ToString());

            metrics.Similarity =
                similarity.SimilarityPercentage;

            metrics.SimilarityMessage =
                similarity.Message;

            // Step 8 - Final Score

            metrics.FinalScore =
                _scoring.Calculate(
                    metrics.Score,
                    metrics.AIScore,
                    metrics.Similarity);

            // Step 9 - Save History

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

            // Step 10 - Cache

            await _cache.SetAsync(
                cacheKey,
                System.Text.Json.JsonSerializer.Serialize(metrics));

            Console.WriteLine("========== PIPELINE END ==========");

            return metrics;
        }
    }
}