using TM_MULTIHEAD_PHISHING_DETECTOR.Models;

public class MHTMEngine
{
    public AnalysisResult Process(string text)
    {
        var result = new AnalysisResult();
        result.OriginalText = text;

        // Run all heads - each returns normalized score (flags/max_flags) between 0.0 and 1.0
        var (score1, t1) = new Head1().Run(text);
        var (score2, t2) = new Head2().Run(text);
        var (score3, t3) = new Head3().Run(text);

        // Store individual normalized scores for display
        result.Head1Score = score1;
        result.Head2Score = score2;
        result.Head3Score = score3;

        result.Head1Triggers = t1;
        result.Head2Triggers = t2;
        result.Head3Triggers = t3;


        // Since each head already returns (flags/max_flags), we just apply weights
        double head1Contribution = score1 * 0.40;
        double head2Contribution = score2 * 0.30;
        double head3Contribution = score3 * 0.30;

        // Sum contributions (this gives 0.0 to 1.0 range)
        double finalNormalizedScore = head1Contribution + head2Contribution + head3Contribution;

        // Convert to percentage (0-100 scale) as specified in paper
        result.ConfidenceScore = finalNormalizedScore * 100;

        // 0-49%: CREDIBLE
        // 50-100%: SUSPICIOUS
        result.Classification = result.ConfidenceScore >= 50
            ? "Suspicious"
            : "Credible";

        return result;
    }
}