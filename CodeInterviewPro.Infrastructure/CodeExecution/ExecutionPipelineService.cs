using CodeInterviewPro.Application.AI;
using CodeInterviewPro.Application.Common.Execution;
using CodeInterviewPro.Application.Interfaces.Repositories;
using CodeInterviewPro.Application.Interfaces.Services;
using CodeInterviewPro.Domain.Entities;
using CodeInterviewPro.Domain.Enums;
using CodeInterviewPro.Infrastructure.CodeExecution.StaticAnalysis;
using CodeInterviewPro.Infrastructure.CodeExecution.StaticAnalysis.Roslyn;
using CodeInterviewPro.Infrastructure.Common;
using CodeInterviewPro.Infrastructure.StaticAnalysis.ESLint;
using CodeInterviewPro.Infrastructure.StaticAnalysis.Go;
using CodeInterviewPro.Infrastructure.StaticAnalysis.Java;
using CodeInterviewPro.Infrastructure.StaticAnalysis.Python;

namespace CodeInterviewPro.Infrastructure.CodeExecution
{
    public class ExecutionPipelineService: IExecutionPipelineService
    {
        private readonly IEnumerable<IStaticAnalyzer> _analyzers;
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
        private readonly IAIFeedbackService _aiFeedback;
        private readonly IFinalFeedbackService _finalFeedback;
        private readonly ICodeRunnerTemplate _template;
        private readonly IBackgroundTaskQueue _backgroundQueue;

        public ExecutionPipelineService(
            IEnumerable<IStaticAnalyzer> analyzers,
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
            IConfidenceService confidence,
            IAIFeedbackService aiFeedback,
            IFinalFeedbackService finalFeedback,
            ICodeRunnerTemplate template,
            IBackgroundTaskQueue backgroundQueue)
        {
            _analyzers = analyzers ?? throw new ArgumentNullException(nameof(analyzers));
            _executionService = executionService ?? throw new ArgumentNullException(nameof(executionService));
            _testCaseService = testCaseService ?? throw new ArgumentNullException(nameof(testCaseService));
            _metricsService = metricsService ?? throw new ArgumentNullException(nameof(metricsService));
            _cache = cache ?? throw new ArgumentNullException(nameof(cache));
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
            _rateLimit = rateLimit ?? throw new ArgumentNullException(nameof(rateLimit));
            _timeout = timeout ?? throw new ArgumentNullException(nameof(timeout));
            _resource = resource ?? throw new ArgumentNullException(nameof(resource));
            _similarity = similarity ?? throw new ArgumentNullException(nameof(similarity));
            _scoring = scoring ?? throw new ArgumentNullException(nameof(scoring));
            _aiIntelligence = aiIntelligence ?? throw new ArgumentNullException(nameof(aiIntelligence));
            _deepAnalysis = deepAnalysis ?? throw new ArgumentNullException(nameof(deepAnalysis));
            _codeBert = codeBert ?? throw new ArgumentNullException(nameof(codeBert));
            _confidence = confidence ?? throw new ArgumentNullException(nameof(confidence));
            _aiFeedback = aiFeedback ?? throw new ArgumentNullException(nameof(aiFeedback));
            _finalFeedback = finalFeedback ?? throw new ArgumentNullException(nameof(finalFeedback));
            _template = template ?? throw new ArgumentNullException(nameof(template));
            _backgroundQueue = backgroundQueue ?? throw new ArgumentNullException(nameof(backgroundQueue));
        }

        public async Task<ExecutionResult> ExecuteAsync(
            string code,
            ProgrammingLanguage language,
            List<TestCase> testCases,
            string methodName)
        {
            if (code == null) throw new ArgumentNullException(nameof(code));

            var stopwatch = System.Diagnostics.Stopwatch.StartNew();

            Console.WriteLine("========== PIPELINE START ==========");

            var rateKey = $"execution_rate:{methodName}";

            var allowed = await _rateLimit.IsAllowedAsync(rateKey, 5, 60);

            if (!allowed)
            {
                throw new Exception("Too many executions. Try again later.");
            }

            if (language == ProgrammingLanguage.CSharp)
            {
                code = _template.WrapCode(code, testCases, methodName);
            }

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

            // Run all applicable analyzers
            foreach (var analyzer in _analyzers)
            {
                if (!analyzer.Supports(language)) continue;

                var compilerErrors = await analyzer.AnalyzeAsync(code);
                if (!string.IsNullOrWhiteSpace(compilerErrors))
                {
                    return new ExecutionResult { AIFeedback = compilerErrors };
                }
            }
          
            List<TestCaseResult> results;
            try
            {
                results = await _resource.ExecuteWithLimits(
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
            }
            catch (Exception ex)
            {
                return new ExecutionResult
                {
                    Total = testCases.Count,
                    Passed = 0,
                    Failed = testCases.Count,
                    Score = 0,
                    FinalScore = 0,
                    AIFeedback = ex.Message
                };
            }

            var metrics = _metricsService.Calculate(results ?? new List<TestCaseResult>()) 
                          ?? new ExecutionResult();

            // AI Cache

            var aiCacheKey =
                $"ai:{code.GetHashCode()}:{language}";

            var cachedAI =
                await _cache.GetAsync(aiCacheKey);

            AIIntelligenceResult aiResult;
            CodeBertResult deepResult;
            CodeBertResult codeBertResult;
            CodeSimilarityResult similarity;

            if (!string.IsNullOrEmpty(cachedAI))
            {
                Console.WriteLine("AI CACHE HIT");

                var cachedAIResult = System.Text.Json.JsonSerializer.Deserialize<AICombinedCache>(cachedAI);

                aiResult = cachedAIResult?.AI ?? new AIIntelligenceResult();
                deepResult = cachedAIResult?.Deep ?? new CodeBertResult();
                codeBertResult = cachedAIResult?.CodeBert ?? new CodeBertResult();
                similarity = cachedAIResult?.Similarity ?? new CodeSimilarityResult();
            }
            else
            {
                var aiTask = Task.Run(() =>
                    _aiIntelligence.Analyze(code));

                var deepTask =
                    _deepAnalysis.AnalyzeAsync(
                        code,
                        language.ToString());

                var codeBertTask =
                    _codeBert.AnalyzeAsync(
                        code,
                        language.ToString());

                var similarityTask =
                    _similarity.CheckSimilarityAsync(
                        code,
                        language.ToString());

                await Task.WhenAll(
                    aiTask,
                    deepTask,
                    codeBertTask,
                    similarityTask);

                aiResult = await aiTask ?? new AIIntelligenceResult();
                deepResult = await deepTask ?? new CodeBertResult();
                codeBertResult = await codeBertTask ?? new CodeBertResult();
                similarity = await similarityTask ?? new CodeSimilarityResult();

                var combined =
                    new AICombinedCache
                    {
                        AI = aiResult,
                        Deep = deepResult,
                        CodeBert = codeBertResult,
                        Similarity = similarity
                    };

                await _cache.SetAsync(
                    aiCacheKey,
                    System.Text.Json.JsonSerializer.Serialize(combined));
            }

            var confidenceScore =
                _confidence.CalculateConfidence(
                    aiResult.FinalScore,
                    deepResult.Score,
                    codeBertResult.Score);
            metrics.AIScore = confidenceScore;

            string chatFeedback =
                await _aiFeedback.GenerateAsync(
                    "Coding Question",
                    "Candidate Solution",
                    code,
                    language.ToString(),
                    codeBertResult.Complexity,
                    confidenceScore);

            metrics.Similarity =
                similarity.SimilarityPercentage;

            metrics.SimilarityMessage =
                similarity.Message;

            metrics.FinalScore =
                _scoring.Calculate(
                    metrics.Score,
                    metrics.AIScore,
                    metrics.Similarity);

            metrics.AIFeedback =
                _finalFeedback.Generate(
                    aiResult.Feedback,
                    deepResult.Feedback,
                    codeBertResult.Feedback,
                    chatFeedback,
                    metrics.Similarity,
                    metrics.FinalScore,
                    codeBertResult.Complexity);
            //metrics.AIScore = confidenceScore;

            //string chatFeedback = "AI feedback processing...";

            //_backgroundQueue.QueueBackgroundWorkItem(
            //    async token =>
            //    {
            //        await _aiFeedback.GenerateAsync(
            //            "Coding Question",
            //            "Candidate Solution",
            //            code,
            //            language.ToString(),
            //            codeBertResult.Complexity,
            //            confidenceScore);

            //        Console.WriteLine("Background Gemini Completed");
            //    });

            //metrics.Similarity =
            //    similarity.SimilarityPercentage;

            //metrics.SimilarityMessage =
            //    similarity.Message;

            //metrics.FinalScore =
            //    _scoring.Calculate(
            //        metrics.Score,
            //        metrics.AIScore,
            //        metrics.Similarity);

            //metrics.AIFeedback =
            //    _finalFeedback.Generate(
            //        aiResult.Feedback,
            //        deepResult.Feedback,
            //        codeBertResult.Feedback,
            //        chatFeedback,
            //        metrics.Similarity,
            //        metrics.FinalScore,
            //        codeBertResult.Complexity);

            metrics.AIComplexity =
                codeBertResult.Complexity;

            //var history =
            //    new ExecutionHistory
            //    {
            //        Id = Guid.NewGuid(),
            //        Code = code,
            //        Language = language.ToString(),
            //        Total = metrics.Total,
            //        Passed = metrics.Passed,
            //        Failed = metrics.Failed,
            //        Score = (int)metrics.Score,
            //        AIScore = metrics.AIScore,
            //        AIFeedback = metrics.AIFeedback,
            //        AIComplexity = metrics.AIComplexity,
            //        FinalScore = metrics.FinalScore,
            //        CreatedAt = DateTime.UtcNow
            //    };

            //await _repository.SaveAsync(history);

            await _cache.SetAsync(
                cacheKey,
                System.Text.Json.JsonSerializer.Serialize(metrics));

            Console.WriteLine("========== PIPELINE END ==========");
            Console.WriteLine($"TOTAL EXECUTION TIME: {stopwatch.ElapsedMilliseconds} ms");


            return metrics;
        }
    }
}