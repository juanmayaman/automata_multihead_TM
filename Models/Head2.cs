using System.Numerics;

namespace TM_MULTIHEAD_PHISHING_DETECTOR.Models
{
    public class Head2 //  Structural patterns (excessive punctuation, unusual capitalization)
    {
        public enum Pattern1HeadState
        {
            q0,          // Start state
            q_Punct,     // Pattern 1 | Punctuations | Excessive Punctuation !!!, ???, !!??!! | Punctuation ay dapat 2+ para ma count
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
            var state = Pattern1HeadState.q0;
            var FinalState = HeadStates.HeadState.q0;
            var triggers = new List<string>();
            var checkCaps = text.Split(new char[] { ' ', '.', ',', '!', '?', ';',':','"','(', ')','-'}, StringSplitOptions.RemoveEmptyEntries); //pattern 2
            double score = 0.0;
            int punctuationCount = 0;
            int countCaps = 0;
            int flagCount = 0; //counter for the second head, each violation

            //for Pattern 1 | Excessive Punctuation and Pattern 3 | Total Punctuation



            //for caps | Pattern 2
            foreach (var word in checkCaps)
            {
                bool isAllCaps = word.All(c => char.IsUpper(c));
                if (word.Length > 3 && isAllCaps && !ExcludedAcronyms.Contains(word))
                {
                    countCaps++; 
                }

                if(countCaps >=2)
                {
                    flagCount++;
                }
            }

            /*switch(FinalState)
            {
                case HeadStates.HeadState.q0:
                    if(punctuationCount >= 2)
                    {
                        FinalState = HeadStates.HeadState.q1;
                        triggers.Add("Excessive Punctuation");
                    }
                    break;
                case HeadStates.HeadState.q1:
                    //stay in q1
                    break;
                default:
                    FinalState = HeadStates.HeadState.q_reject;
                    break;
            }*/


            return (score, triggers);
        }


   
    }
}
