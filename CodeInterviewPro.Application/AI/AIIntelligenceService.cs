using CodeInterviewPro.Domain.Entities;
using CodeInterviewPro.Domain.Enums;

namespace CodeInterviewPro.Application.AI
{
    public class AIIntelligenceService
    {
        private readonly AISemanticAnalyzer _semantic;
        private readonly CodeSmellAnalyzer _smell;
        private readonly PatternAnalyzer _pattern;

        public AIIntelligenceService()
        {
            _semantic = new AISemanticAnalyzer();
            _smell = new CodeSmellAnalyzer();
            _pattern = new PatternAnalyzer();
        }

        public AIIntelligenceResult Analyze(string code)
        {
            var semantic =
                _semantic.Analyze(code);

            var smell =
                _smell.Analyze(code);

            var pattern =
                _pattern.Analyze(code);

            var score =
                CalculateScore(
                    semantic.Score,
                    smell.Score,
                    pattern.Score);

            return new AIIntelligenceResult
            {
                Semantic = semantic,
                CodeSmell = smell,
                Pattern = pattern,
                FinalScore = score,
                Feedback =
                    GenerateFeedback(
                        semantic,
                        smell,
                        pattern)
            };
        }

        private int CalculateScore(
            int semantic,
            int smell,
            int pattern)
        {
            return (semantic + smell + pattern) / 3;
        }

        private string GenerateFeedback(
            SemanticAnalysisResult semantic,
            CodeSmellResult smell,
            PatternAnalysisResult pattern)
        {
            return
                semantic.Feedback +
                "\n" +
                smell.Feedback +
                "\n" +
                pattern.Feedback;
        }
    }

    public class AIIntelligenceResult
    {
        public SemanticAnalysisResult Semantic { get; set; }

        public CodeSmellResult CodeSmell { get; set; }

        public PatternAnalysisResult Pattern { get; set; }

        public int FinalScore { get; set; }

        public string Feedback { get; set; }
    }
}