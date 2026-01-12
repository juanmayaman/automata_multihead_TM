using System;
using System.Collections.Generic;

namespace TM_MULTIHEAD_PHISHING_DETECTOR.Models
{
    public class Head3 // Repetition detection
    {
        private static readonly HashSet<string> StopWords = new(StringComparer.OrdinalIgnoreCase)
        {
            "the", "and", "is", "a", "an", "to", "with",
            "of", "in", "for", "on", "at", "this", "that"
        };

        // Automata state for word building
        private enum WordState
        {
            ReadingWord,
            AtBoundary
        }

        public (double score, List<string> triggers) Run(string text)
        {
            var triggers = new List<string>();
            var finalState = HeadStates.HeadState.q0;
            int flagCount = 0;

            if (string.IsNullOrWhiteSpace(text))
            {
                finalState = HeadStates.HeadState.q_reject;
                return (0.0, triggers);
            }

            // Word building with state transitions
            var wordState = WordState.AtBoundary;
            List<string> words = new List<string>();
            string currentWord = "";

            // AUTOMATA: 
            for (int i = 0; i < text.Length; i++)
            {
                char c = text[i];

                // STATE TRANSITION 
                if (char.IsLetter(c))
                {
                    // Transition to ReadingWord state
                    wordState = WordState.ReadingWord;
                    currentWord += char.ToLower(c);
                }
                else
                {
                    // Transition to AtBoundary state
                    if (wordState == WordState.ReadingWord && currentWord.Length > 0)
                    {
                        words.Add(currentWord);
                        currentWord = "";
                    }
                    wordState = WordState.AtBoundary;
                }
            }

            // Add last word if still reading
            if (wordState == WordState.ReadingWord && currentWord.Length > 0)
            {
                words.Add(currentWord);
            }

            // WORD-LEVEL REPETITION DETECTION
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
                    finalState = HeadStates.HeadState.q2_accept;
                    flagCount = 1;
                    break;
                }
            }

            // BIGRAM-LEVEL REPETITION DETECTION
            if (flagCount == 0 && words.Count >= 2)
            {
                var bigramFreq = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);

                for (int i = 0; i < words.Count - 1; i++)
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
                        finalState = HeadStates.HeadState.q2_accept;
                        flagCount = 1;
                        break;
                    }
                }
            }

            // FINAL STATE
            if (flagCount == 0)
                finalState = HeadStates.HeadState.q_reject;

            double normalizedScore = (double)flagCount / 1.0;

            return (normalizedScore, triggers);
        }
    }
}