using System;
using System.Collections.Generic;

namespace TM_MULTIHEAD_PHISHING_DETECTOR.Models
{
    public class Head2 // Structural patterns (excessive punctuation, unusual capitalization)
    {
        // Automata states
        public enum PunctState
        {
            q0,          // Start / no punctuation
            q_Punct      // Inside punctuation streak
        }

        public enum WordState
        {
            ReadingWord,
            AtBoundary
        }

        private HashSet<string> ExcludedAcronyms = new()
        {
            "USA", "UK", "UN", "EU", "ASEAN", "NATO", "WHO", "IMF", "WTO", "UNICEF",
            "FBI", "CIA", "DOJ", "PNP", "AFP", "NBI", "DILG", "CHED", "DEPED", "DOH",
            "COVID", "COVID-19", "SARS", "HIV", "AIDS", "CDC", "FDA",
            "CNN", "BBC", "GMA", "ABS", "NASA", "HBO",
            "DND", "PAF", "PCG", "NDRRMC"
        };

        public (double score, List<string> triggers) Run(string text)
        {
            var triggers = new List<string>();
            int flagCount = 0;
            var finalState = HeadStates.HeadState.q0;

            // Pattern 1 & 3: Punctuation tracking
            var punctState = PunctState.q0;
            int punctuationCount = 0;
            int punctuationStreak = 0;
            bool pattern1Detected = false;
            bool pattern3Detected = false;

            // Pattern 2: ALL CAPS tracking
            var wordState = WordState.AtBoundary;
            string currentWord = "";
            int countCaps = 0;
            bool pattern2Detected = false;

            // AUTOMATA: 
            for (int i = 0; i < text.Length; i++)
            {
                char c = text[i];

                // === PATTERN 1 & 3: Punctuation DFA (STATE TRANSITION per char) ===
                if (char.IsPunctuation(c))
                {
                    // Transition to punctuation state
                    if (punctState == PunctState.q0)
                    {
                        punctState = PunctState.q_Punct;
                        punctuationStreak = 1;
                    }
                    else // already in q_Punct
                    {
                        punctuationStreak++;
                    }
                    punctuationCount++;

                    // Check for consecutive punctuation pattern
                    if (punctuationStreak >= 3 && !pattern1Detected)
                    {
                        pattern1Detected = true;
                    }
                }
                else
                {
                    // Transition out of punctuation state
                    if (punctState == PunctState.q_Punct)
                    {
                        punctState = PunctState.q0;
                        punctuationStreak = 0;
                    }
                }

                // === PATTERN 2: ALL CAPS DFA  ===
                if (char.IsLetter(c))
                {
                    // Transition to reading word
                    wordState = WordState.ReadingWord;
                    currentWord += c;
                }
                else
                {
                    // Transition to boundary - check complete word
                    if (wordState == WordState.ReadingWord && currentWord.Length >= 3)
                    {
                        bool isAllCaps = true;
                        for (int j = 0; j < currentWord.Length; j++)
                        {
                            if (!char.IsUpper(currentWord[j]))
                            {
                                isAllCaps = false;
                                break;
                            }
                        }

                        if (isAllCaps && !ExcludedAcronyms.Contains(currentWord))
                        {
                            countCaps++;
                            if (countCaps >= 2)
                                pattern2Detected = true;
                        }
                    }
                    currentWord = "";
                    wordState = WordState.AtBoundary;
                }
            }

            // Check last word
            if (wordState == WordState.ReadingWord && currentWord.Length >= 3)
            {
                bool isAllCaps = true;
                for (int j = 0; j < currentWord.Length; j++)
                {
                    if (!char.IsUpper(currentWord[j]))
                    {
                        isAllCaps = false;
                        break;
                    }
                }

                if (isAllCaps && !ExcludedAcronyms.Contains(currentWord))
                {
                    countCaps++;
                    if (countCaps >= 2)
                        pattern2Detected = true;
                }
            }

            // Pattern 3: total punctuation threshold
            if (punctuationCount >= 5)
                pattern3Detected = true;

            // Count flags and add triggers
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

            // FINAL STATE TRANSITION based on flag count
            if (flagCount == 0)
                finalState = HeadStates.HeadState.q_reject;
            else if (flagCount == 1)
                finalState = HeadStates.HeadState.q1;
            else if (flagCount >= 2)
                finalState = HeadStates.HeadState.q2_accept;

            double normalizedScore = Math.Min((double)flagCount / 2.0, 1.0);

            return (normalizedScore, triggers);
        }
    }
}