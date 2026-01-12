namespace TM_MULTIHEAD_PHISHING_DETECTOR.Models
{
    public class Head1 // Emotional and lexical cues
    {
        private HashSet<string> SuspiciousWords = new()
        {
            "shocking", "breaking", "unbelievable", "bombshell", "exclusive",
            "exposed", "urgent", "alert", "must", "never", "always",
            "amazing", "incredible", "insane", "crazy"
        };

        // Automata states for word building
        private enum WordBuildState
        {
            ReadingWord,      // Currently reading letters
            AtBoundary        // At word boundary (space, punctuation, etc)
        }

        public (double score, List<string> triggers) Run(string text)
        {
            var triggers = new List<string>();
            int flagCount = 0;
            var finalState = HeadStates.HeadState.q0;

            // Word building state
            var wordState = WordBuildState.AtBoundary;
            string currentWord = "";

            // AUTOMATA:
            for (int i = 0; i < text.Length; i++)
            {
                char c = text[i];


                if (char.IsLetter(c))
                {
                    // Transition to ReadingWord state
                    wordState = WordBuildState.ReadingWord;
                    currentWord += char.ToLower(c);
                }
                else
                {
                    // Transition to AtBoundary state
                    if (wordState == WordBuildState.ReadingWord)
                    {
                        // Complete word - check if suspicious
                        if (currentWord.Length > 0 && SuspiciousWords.Contains(currentWord))
                        {
                            triggers.Add(currentWord);
                            flagCount++;

                            // STATE TRANSITION based on flag count
                            if (flagCount == 1)
                                finalState = HeadStates.HeadState.q1;
                            else if (flagCount >= 2)
                                finalState = HeadStates.HeadState.q2_accept;
                        }
                        currentWord = "";
                    }
                    wordState = WordBuildState.AtBoundary;
                }
            }

            // Process last word if exists
            if (wordState == WordBuildState.ReadingWord && currentWord.Length > 0)
            {
                if (SuspiciousWords.Contains(currentWord))
                {
                    triggers.Add(currentWord);
                    flagCount++;

                    if (flagCount == 1)
                        finalState = HeadStates.HeadState.q1;
                    else if (flagCount >= 2)
                        finalState = HeadStates.HeadState.q2_accept;
                }
            }

            // Final state determination
            if (flagCount < 2 && finalState != HeadStates.HeadState.q2_accept)
                finalState = HeadStates.HeadState.q_reject;

            double normalizedScore = Math.Min((double)flagCount / 2.0, 1.0);

            return (normalizedScore, triggers);
        }
    }
}