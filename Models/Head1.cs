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

        public (double score, List<string> triggers) Run(string text)
        {
            // DFA-like simulation
            var triggers = new List<string>();
            int flagCount = 0;
            var state = HeadStates.HeadState.q0;

            var words = text.ToLower().Split(new char[] { ' ', '.', ',', '!', '?' }, StringSplitOptions.RemoveEmptyEntries);

            foreach (var word in words)
            {
                if (SuspiciousWords.Contains(word))
                {
                    triggers.Add(word);
                    flagCount++;

                    if (flagCount == 1)
                        state = HeadStates.HeadState.q1;
                    else if (flagCount >= 2)
                        state = HeadStates.HeadState.q2_accept;
                }
            }

            // Final state check
            if (flagCount < 2)
                state = HeadStates.HeadState.q_reject;

            double weight = state == HeadStates.HeadState.q2_accept
                ? 0.40
                : state == HeadStates.HeadState.q1
                    ? 0.20
                    : 0.05;

            double score = weight * triggers.Count;

            return (score, triggers);
        }
    }
}
