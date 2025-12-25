namespace TM_MULTIHEAD_PHISHING_DETECTOR.Models
{
    public class Head2 //  Structural patterns (excessive punctuation, unusual capitalization)
    {
        public enum SecondHeadState
        {
            q0,          // Start state
            q_Punct,     // Pattern 1 | Punctuations | Excessive Punctuation !!!, ???, !!??!! | Punctuation ay dapat 2+ para ma count
        }
 
    public (double score, List<string> triggers) Run(string text)
        {

            // DFA-like simulation
            var state = SecondHeadState.q0;
            var FinalState = HeadStates.HeadState.q0;
            var triggers = new List<string>();
            var words = text;
            var checkCaps = text.Split(new char[] { ' ', '.', ',', '!', '?', ';',':','"','(', ')','-'}, StringSplitOptions.RemoveEmptyEntries); //pattern 2
            double score = 0.0;
            int punctuationCount = 0;
            int capsCount = 0;

            //for punctuations and state 1
     


            //for caps
            foreach (var word in words)
            {
                switch (state)
                {
                   
                }
            }
            




            return (score, triggers);
        }


   
    }
}
