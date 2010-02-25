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
    }
}
