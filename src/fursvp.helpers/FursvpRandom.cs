namespace Fursvp.Helpers
{
    using System;
    using System.Linq;

    /// <summary>
    /// Helper methods for random behavior.
    /// </summary>
    public class FursvpRandom
    {
        /// <summary>
        /// No lucky guesses random code.
        /// There are 414,208 possible combinations. An attacker knowing this algorithm would have a 1 in 82,841.6 chance (0.001207%) of getting 1 guess correct out of 5 guesses.
        /// </summary>
        /// <returns>A six digit numeric code that is easy to remember and copy by hand, but hard to guess randomly. Good for verification codes.</returns>
        public static string CopyableButHardToGuessCode()
        {
            string code;
            do
            {
                code = new Random().Next(100236, 998752).ToString();
            }
            while (!IsSturdy(code));

            return code;
        }

        private static bool IsSturdy(string code)
        {
            var codeArray = code.Select(c => Convert.ToInt32(char.GetNumericValue(c))).ToArray();
            return DigitOccurrence(codeArray) > 4 && DigitSpread(codeArray) > 3;
        }

        private static int DigitSpread(int[] code)
        {
            int[] spreadCounts = new int[10];
            for (int i = 0; i < code.Length - 1; i++)
            {
                spreadCounts[Math.Abs(code[i] - code[i + 1])]++;
            }

            return spreadCounts.Count(c => c > 0);
        }

        private static int DigitOccurrence(int[] code)
        {
            int[] digitCounts = new int[10];
            foreach (char c in code)
            {
                digitCounts[c]++;
            }

            return digitCounts.Count(c => c > 0);
        }
    }
}
