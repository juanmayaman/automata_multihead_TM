using TM_MULTIHEAD_PHISHING_DETECTOR.Models;

public class MHTMEngine
{
    public AnalysisResult Process(string text)
    {
        var result = new AnalysisResult();  
        result.OriginalText = text;

        var (score1, t1) = new Head1().Run(text);
        var (score2, t2) = new Head2().Run(text);
        var (score3, t3) = new Head3().Run(text);

        result.Head1Score = score1;
        result.Head2Score = score2;
        result.Head3Score = score3;

        result.Head1Triggers = t1;
        result.Head2Triggers = t2;
        result.Head3Triggers = t3;

        // Weighted scoring
        double finalScore =
            (score1 * 0.40)+
        (score2 * 0.30) +
        (score3 * 0.30);

        result.ConfidenceScore = finalScore;

        result.Classification = finalScore >= 0.50
            ? "Suspicious"
            : "Credible";

        return result;
    }
}
