namespace TM_MULTIHEAD_PHISHING_DETECTOR.Models
{
    
    public class Head1 //emotional and lexical cues (exaggerated expressions, emotionally charged language)
    {
        private HashSet<string> SuspiciousWords = new() {
        "shocking", "breaking", "unbelievable", "bombshell", "exclusive",
        "exposed", "urgent", "alert", "must", "never", "always","amazing","incredible","insane","crazy"
    };

        public (double score, List<string> triggers) Run(string text)
        {
            // DFA-like simulation

            var state = HeadStates.HeadState.q0;
            var triggers = new List<string>();
            var words = text.ToLower().Split(new char[] { ' ', '.', ',', '!', '?' }, StringSplitOptions.RemoveEmptyEntries);

            double score = 0.0;
            foreach (var word in words)
            {
                switch (state)
                {
                    case HeadStates.HeadState.q0:
                        if (SuspiciousWords.Contains(word))
                        {
                            state = HeadStates.HeadState.q1;
                            triggers.Add(word);
                        }
                        break;
                    case HeadStates.HeadState.q1:
                        // Stay in q1 for additional suspicious words
                        if (SuspiciousWords.Contains(word))
                        {
                            triggers.Add(word);
                        }
                        else
                        {
                            state = HeadStates.HeadState.q2_accept;
                        }
                        break;
                    case HeadStates.HeadState.q2_accept:
                        // Once accepted, remain accepted
                        break;

                    default:
                        state = HeadStates.HeadState.q_reject;
                        break;
                }
            }

            score = .40 * triggers.Count;

            return (score, triggers);
        }
    }
}
