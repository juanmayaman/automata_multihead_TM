using System;
using System.Collections.Generic;

namespace TM_MULTIHEAD_PHISHING_DETECTOR.Models
{
    public class Head3
    {
        private static readonly HashSet<string> StopWords = new(StringComparer.OrdinalIgnoreCase)
        {
            "the", "and", "is", "a", "an", "to", "with",
            "of", "in", "for", "on", "at", "this", "that"
        };

        public (double score, List<string> triggers) Run(string text)
        {
            var triggers = new List<string>();
            var state = HeadStates.HeadState.q0;
            int flagCount = 0;

            if (string.IsNullOrWhiteSpace(text))
            {
                state = HeadStates.HeadState.q_reject;
                return (0.0, triggers);
            }

            // --- Preprocessing ---
            var separators = new char[] { ' ', '\t', '\n', '\r', '.', ',', '!', '?', ';', ':', '\"', '\'', '(', ')', '-', '/' };
            var words = text
                .ToLowerInvariant()
                .Split(separators, StringSplitOptions.RemoveEmptyEntries);

            // --- WORD-LEVEL REPETITION ---
            var wordFreq = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);

            foreach (var word in words)
            {
                if (StopWords.Contains(word))
                    continue;

                if (!wordFreq.ContainsKey(word))
                    wordFreq[word] = 1;
                else
                    wordFreq[word]++;

                if (wordFreq[word] >= 3)
                {
                    triggers.Add($"\"{word}\" repeated {wordFreq[word]} times");
                    state = HeadStates.HeadState.q2_accept;
                    flagCount = 1;
                    // Don't return yet, continue to check for bigrams
                    break;
                }
            }

            // --- BIGRAM-LEVEL REPETITION ---
            if (words.Length >= 2)
            {
                var bigramFreq = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);

                for (int i = 0; i < words.Length - 1; i++)
                {
                    if (StopWords.Contains(words[i]) || StopWords.Contains(words[i + 1]))
                        continue;

                    string bigram = words[i] + " " + words[i + 1];

                    if (!bigramFreq.ContainsKey(bigram))
                        bigramFreq[bigram] = 1;
                    else
                        bigramFreq[bigram]++;

                    if (bigramFreq[bigram] >= 2)
                    {
                        triggers.Add($"\"{bigram}\" repeated {bigramFreq[bigram]} times");
                        state = HeadStates.HeadState.q2_accept;
                        flagCount = 1;
                        break;
                    }
                }
            }

            // --- FINAL STATE ---
            if (flagCount == 0)
                state = HeadStates.HeadState.q_reject;


            double normalizedScore = (double)flagCount / 1.0;

            return (normalizedScore, triggers);
        }
    }
}