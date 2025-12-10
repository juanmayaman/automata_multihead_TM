namespace TM_MULTIHEAD_PHISHING_DETECTOR.Models
{
    public class HeadStates
    {
        public enum HeadState
        {
            q0,          // Start state
            q1,          // One suspicious indicator found
            q2_accept,   // Suspicious
            q_reject     // Credible
        }
    }
}
