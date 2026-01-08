namespace TM_MULTIHEAD_PHISHING_DETECTOR.Models
{
    public class AnalysisResult
    {
        public string OriginalText { get; set; }
        public string ProcessedText { get; set; }
        public string Classification { get; set; } // Credible / Suspicious
        public double ConfidenceScore { get; set; }

        // Per-head data:
        public List<string> Head1Triggers { get; set; }
        public List<string> Head2Triggers { get; set; }
        public List<string> Head3Triggers { get; set; }

        // Scores
        public double Head1Score { get; set; }
        public double Head2Score { get; set; }
        public double Head3Score { get; set; }
    }
}
