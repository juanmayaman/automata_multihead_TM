using System;
using System.Collections.Generic;
using System.Linq;

namespace TM_MULTIHEAD_PHISHING_DETECTOR.Models
{
    public class Head2 // Structural patterns (excessive punctuation, unusual capitalization)
    {
        public enum Pattern1HeadState
        {
            q0,          // Start / no punctuation
            q_Punct      // Inside punctuation streak
        }

        private HashSet<string> ExcludedAcronyms = new()
        {
            // Countries & international orgs
            "USA", "UK", "UN", "EU", "ASEAN", "NATO", "WHO", "IMF", "WTO", "UNICEF",

            // Government & law enforcement
            "FBI", "CIA", "DOJ", "PNP", "AFP", "NBI", "DILG", "CHED", "DEPED", "DOH",

            // Health & science
            "COVID", "COVID-19", "SARS", "HIV", "AIDS", "CDC", "FDA",

            // Media & institutions
            "CNN", "BBC", "GMA", "ABS", "NASA", "HBO",

            // Military / disaster response (PH context)
            "DND", "PAF", "PCG", "NDRRMC"
        };

        public (double score, List<string> triggers) Run(string text)
        {
            // DFA state
            var state = Pattern1HeadState.q0;
            var FinalState = HeadStates.HeadState.q0;

            // Outputs
            var triggers = new List<string>();

            // Counters
            int punctuationCount = 0;        // Pattern 3
            int punctuationStreak = 0;       // Pattern 1
            int countCaps = 0;               // Pattern 2
            int flagCount = 0;

            // Pattern flags
            bool pattern1Detected = false;   // Excessive consecutive punctuation
            bool pattern2Detected = false;   // Multiple ALL CAPS words
            bool pattern3Detected = false;   // High total punctuation

            // Pattern 1 & Pattern 3 (DFA)
            for (int i = 0; i < text.Length; i++)
            {
                char currentChar = text[i];

                switch (state)
                {
                    case Pattern1HeadState.q0:
                        if (char.IsPunctuation(currentChar))
                        {
                            punctuationCount++;
                            punctuationStreak = 1;
                            state = Pattern1HeadState.q_Punct;
                        }
                        break;

                    case Pattern1HeadState.q_Punct:
                        if (char.IsPunctuation(currentChar))
                        {
                            punctuationCount++;
                            punctuationStreak++;
                        }
                        else
                        {
                            punctuationStreak = 0;
                            state = Pattern1HeadState.q0;
                        }
                        break;
                }

                // Detect 1 run of 3+ consecutive punctuation
                if (punctuationStreak >= 3 && !pattern1Detected)
                {
                    pattern1Detected = true;
                    punctuationStreak = 0; // consume the run
                }
            }

            // Pattern 3: total punctuation threshold
            if (punctuationCount >= 5)
                pattern3Detected = true;

            // Pattern 2 (ALL CAPS words)
            var words = text.Split(
                new char[] { ' ', '.', ',', '!', '?', ';', ':', '"', '(', ')', '-' },
                StringSplitOptions.RemoveEmptyEntries
            );

            foreach (var word in words)
            {
                bool isAllCaps = word.All(c => char.IsUpper(c));

                if (word.Length > 3 && isAllCaps && !ExcludedAcronyms.Contains(word))
                {
                    countCaps++;
                }

                if (countCaps >= 2)
                {
                    pattern2Detected = true;
                    break;
                }
            }

            // Flag counting + triggers
            if (pattern1Detected)
            {
                flagCount++;
                triggers.Add("Excessive consecutive punctuation");
            }

            if (pattern2Detected)
            {
                flagCount++;
                triggers.Add("Multiple ALL-CAPS words");
            }

            if (pattern3Detected)
            {
                flagCount++;
                triggers.Add("High overall punctuation usage");
            }

            if (flagCount == 0)
                FinalState = HeadStates.HeadState.q_reject;    // credible
            else if (flagCount == 1)
                FinalState = HeadStates.HeadState.q1;         // intermediate
            else if (flagCount >= 2)
                FinalState = HeadStates.HeadState.q2_accept;  // suspicious


            double normalizedScore = Math.Min((double)flagCount / 2.0, 1.0);

            return (normalizedScore, triggers);
        }
    }
}