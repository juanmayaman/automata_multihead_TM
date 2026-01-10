using System.Text.RegularExpressions;
using TM_MULTIHEAD_PHISHING_DETECTOR.Models;

public class MHTMEngine
{
    public async Task<AnalysisResult> ProcessAsync(string text)
    {
        var result = new AnalysisResult();

        result.OriginalText = text;

        string cleanText = PreprocessText(text);
        result.ProcessedText = cleanText;

        // Run heads in parallel
        var head1Task = Task.Run(() => new Head1().Run(cleanText));
        var head2Task = Task.Run(() => new Head2().Run(cleanText));
        var head3Task = Task.Run(() => new Head3().Run(cleanText));

        await Task.WhenAll(head1Task, head2Task, head3Task);

        var (score1, t1) = head1Task.Result;
        var (score2, t2) = head2Task.Result;
        var (score3, t3) = head3Task.Result;

        result.Head1Score = score1;
        result.Head2Score = score2;
        result.Head3Score = score3;

        result.Head1Triggers = t1;
        result.Head2Triggers = t2;
        result.Head3Triggers = t3;

        double finalNormalizedScore =
            (score1 * 0.40) +
            (score2 * 0.30) +
            (score3 * 0.30);

        result.ConfidenceScore = finalNormalizedScore * 100;

        // 0 - 49 = CREDIBLE
        // 50 - 100 = SUSPICIOUS
        result.Classification = result.ConfidenceScore >= 50
            ? "Suspicious"
            : "Credible";

        return result;
    }

    // 🔹 Centralized preprocessing logic
    private string PreprocessText(string text)
    {
        if (string.IsNullOrWhiteSpace(text))
            return string.Empty;

        // Remove URLs
        text = Regex.Replace(
            text,
            @"https?:\/\/\S+|www\.\S+",
            "",
            RegexOptions.IgnoreCase
        );

        // Remove numbers
        text = Regex.Replace(text, @"\d+", "");

        // Remove emojis and special unicode symbols
        text = Regex.Replace(
            text,
            @"[\p{Cs}\p{So}\p{Sk}]+",
            ""
        );

        // Normalize whitespace
        text = Regex.Replace(text, @"\s+", " ").Trim();

        return text;
    }
}
