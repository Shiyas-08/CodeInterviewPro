using Moq;
using Xunit;
using CodeInterviewPro.Infrastructure.CodeExecution;
using CodeInterviewPro.Application.Interfaces.Services;
using CodeInterviewPro.Application.Interfaces.Repositories;
using CodeInterviewPro.Application.Common.Execution;
using CodeInterviewPro.Application.AI;
using CodeInterviewPro.Domain.Enums;
using CodeInterviewPro.Domain.Entities;
using CodeInterviewPro.Infrastructure.Cache;

namespace CodeInterviewPro.UnitTests.Services
{
    public class ExecutionPipelineServiceTests
    {
        // We use real instances for these because they are not easily mockable without interfaces
        private readonly MultiLanguageExecutionService _realExecutionService;
        private readonly TestCaseExecutionService _realTestCaseService;
        private readonly AIIntelligenceService _realAiIntelligence;

        private readonly Mock<IMetricsService> _mockMetricsService;
        private readonly Mock<IExecutionCacheService> _mockCache;
        private readonly Mock<IExecutionHistoryRepository> _mockRepository;
        private readonly Mock<IRateLimitService> _mockRateLimit;
        private readonly Mock<IExecutionTimeoutService> _mockTimeout;
        private readonly Mock<IExecutionResourceService> _mockResource;
        private readonly Mock<ICodeSimilarityService> _mockSimilarity;
        private readonly Mock<IScoringService> _mockScoring;
        private readonly Mock<IDeepAnalysisService> _mockDeepAnalysis;
        private readonly Mock<ICodeBertService> _mockCodeBert;
        private readonly Mock<IConfidenceService> _mockConfidence;
        private readonly Mock<IAIFeedbackService> _mockAiFeedback;
        private readonly Mock<IFinalFeedbackService> _mockFinalFeedback;
        private readonly Mock<ICodeRunnerTemplate> _mockTemplate;
        private readonly Mock<IBackgroundTaskQueue> _mockBackgroundQueue;

        private readonly ExecutionPipelineService _sut;

        public ExecutionPipelineServiceTests()
        {
            // Initialize dependencies for classes we can't easily mock
            var mockFactory = new Mock<ILanguageExecutionFactory>();
            _realExecutionService = new MultiLanguageExecutionService(mockFactory.Object);
            _realTestCaseService = new TestCaseExecutionService(_realExecutionService);
            _realAiIntelligence = new AIIntelligenceService();

            // Initialize all other mocks
            _mockMetricsService = new Mock<IMetricsService>();
            _mockCache = new Mock<IExecutionCacheService>();
            _mockRepository = new Mock<IExecutionHistoryRepository>();
            _mockRateLimit = new Mock<IRateLimitService>();
            _mockTimeout = new Mock<IExecutionTimeoutService>();
            _mockResource = new Mock<IExecutionResourceService>();
            _mockSimilarity = new Mock<ICodeSimilarityService>();
            _mockScoring = new Mock<IScoringService>();

            _mockDeepAnalysis = new Mock<IDeepAnalysisService>();
            _mockCodeBert = new Mock<ICodeBertService>();
            _mockConfidence = new Mock<IConfidenceService>();
            _mockAiFeedback = new Mock<IAIFeedbackService>();
            _mockFinalFeedback = new Mock<IFinalFeedbackService>();
            _mockTemplate = new Mock<ICodeRunnerTemplate>();
            _mockBackgroundQueue = new Mock<IBackgroundTaskQueue>();

            _sut = new ExecutionPipelineService(
                new List<IStaticAnalyzer>(),
                _realExecutionService,
                _realTestCaseService,
                _mockMetricsService.Object,
                _mockCache.Object,
                _mockRepository.Object,
                _mockRateLimit.Object,
                _mockTimeout.Object,
                _mockResource.Object,
                _mockSimilarity.Object,
                _mockScoring.Object,
                _realAiIntelligence,
                _mockDeepAnalysis.Object,
                _mockCodeBert.Object,
                _mockConfidence.Object,
                _mockAiFeedback.Object,
                _mockFinalFeedback.Object,
                _mockTemplate.Object,
                _mockBackgroundQueue.Object
            );
        }

        [Fact]
        public async Task ExecuteAsync_ShouldThrowException_WhenRateLimitExceeded()
        {
            // Arrange
            _mockRateLimit.Setup(x => x.IsAllowedAsync(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>()))
                .ReturnsAsync(false);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<System.Exception>(() => 
                _sut.ExecuteAsync("code", ProgrammingLanguage.CSharp, new List<TestCase>(), "method"));
            
            Assert.Equal("Too many executions. Try again later.", exception.Message);
        }

        [Fact]
        public async Task ExecuteAsync_ShouldReturnCachedResult_WhenCacheExists()
        {
            // Arrange
            _mockRateLimit.Setup(x => x.IsAllowedAsync(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>()))
                .ReturnsAsync(true);
            
            var cachedResult = new ExecutionResult { Score = 95, Passed = 5 };
            _mockCache.Setup(x => x.GetAsync(It.IsAny<string>()))
                .ReturnsAsync(System.Text.Json.JsonSerializer.Serialize(cachedResult));

            // Act
            var result = await _sut.ExecuteAsync("code", ProgrammingLanguage.CSharp, new List<TestCase>(), "method");

            // Assert
            Assert.NotNull(result);
            Assert.Equal(95, result.Score);
        }

        [Fact]
        public async Task ExecuteAsync_ShouldProcessFullPipeline_WhenNoCache()
        {
            // Arrange
            _mockRateLimit.Setup(x => x.IsAllowedAsync(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>()))
                .ReturnsAsync(true);

            // Both cache calls return null (cache miss)
            _mockCache.Setup(x => x.GetAsync(It.IsAny<string>()))
                .ReturnsAsync((string)null);
            _mockCache.Setup(x => x.SetAsync(It.IsAny<string>(), It.IsAny<string>()))
                .Returns(Task.CompletedTask);

            // Critical: template.WrapCode must not return null
            _mockTemplate.Setup(x => x.WrapCode(It.IsAny<string>(), It.IsAny<List<TestCase>>(), It.IsAny<string>()))
                .Returns("public class Program { public static void Main() {} }");

            var testCaseResults = new List<TestCaseResult> { new TestCaseResult { Passed = true } };

            // Mock resource to return results directly (bypasses timeout chain in tests)
            _mockResource.Setup(x => x.ExecuteWithLimits(
                    It.IsAny<Func<Task<List<TestCaseResult>>>>(),
                    It.IsAny<int>(),
                    It.IsAny<int>()))
                .ReturnsAsync(testCaseResults);

            _mockMetricsService.Setup(x => x.Calculate(It.IsAny<List<TestCaseResult>>()))
                .Returns(new ExecutionResult { Passed = 1, Total = 1, Score = 100 });

            _mockDeepAnalysis.Setup(x => x.AnalyzeAsync(It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(new CodeBertResult { Score = 85, Feedback = "Optimal" });

            _mockCodeBert.Setup(x => x.AnalyzeAsync(It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(new CodeBertResult { Score = 80, Feedback = "Good" });

            _mockSimilarity.Setup(x => x.CheckSimilarityAsync(It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(new CodeSimilarityResult { SimilarityPercentage = 10 });

            _mockScoring.Setup(x => x.Calculate(It.IsAny<double>(), It.IsAny<double>(), It.IsAny<double>()))
                .Returns(92);

            _mockConfidence.Setup(x => x.CalculateConfidence(It.IsAny<double>(), It.IsAny<double>(), It.IsAny<double>()))
                .Returns(88);

            _mockFinalFeedback.Setup(x => x.Generate(
                    It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(),
                    It.IsAny<string>(), It.IsAny<double>(), It.IsAny<int>(), It.IsAny<string>()))
                .Returns("Great work!");

            _mockAiFeedback.Setup(x => x.GenerateAsync(
                    It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(),
                    It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>()))
                .ReturnsAsync("Good job!");

            // Act
            var result = await _sut.ExecuteAsync("some code", ProgrammingLanguage.CSharp, new List<TestCase>(), "method");

            // Assert
            Assert.NotNull(result);
        }
    }
}
