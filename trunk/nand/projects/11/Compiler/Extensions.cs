using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Interfaces;

namespace Compiler
{
    public static class Extensions
    {
        /// <summary>
        /// Takes a list of string pairs and formats nicely for string output.
        /// Probably only used for testing.
        /// </summary>
        /// <param name="stringPairList">The string pair list.</param>
        /// <returns></returns>
        public static string PairListToString(this IList<Pair<string, string>> stringPairList)
        {
            StringBuilder strBuilder = new StringBuilder();
            foreach (Pair<string, string> pair in stringPairList)
            {
                strBuilder.Append(pair.ToString());
                strBuilder.Append(Environment.NewLine);
            }

            return strBuilder.ToString();
        }

        public static Pair<string, string> PeekSafely(this Stack<Pair<string,string>> stack)
        {
            Pair<string, string> result = null;
            if (stack != null && stack.Count > 0)
            {
                result = stack.Peek();
            }
            else
            {
                result = new Pair<string, string>();
                result.Value1 = "ERROR WHEN PEEKING";
                result.Value2 = "ERROR WHEN PEEKING";
            }

            return result;
        }

        public static bool PopIfExists(this Stack<Pair<string, string>> stack, string target)
        {
            bool result = false;
            if (stack.PeekSafely().Value2 == target)
            {
                stack.Pop();
                result = true;
            }
            return result;
        }

        private static bool IsIntegerConstant(this Stack<Pair<string, string>> stack)
        {
            Pair<string, string> token = stack.PeekSafely();

            if (token != null)
            {
                int number = 0;
                return int.TryParse(token.Value2, out number);
            }

            return false;
        }
    }
}
