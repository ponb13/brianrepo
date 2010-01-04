using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Interfaces;
using System.IO;
using System.Text.RegularExpressions;

namespace States
{
    public class NewToken : IState
    {
        #region singleton logic
        private static IState state = new NewToken();

        private NewToken()
        {
        }

        public static IState Instance()
        {
            return state;
        }
        #endregion
        
        #region IState Members

        public void Read(ITokenizer tokenizer)
        {
            // we're in a new token so clear the previously read chars
            tokenizer.TokenCharacters = new StringBuilder();

            this.ChangeState(tokenizer);
        }

        private void ChangeState(ITokenizer tokenizer)
        {
            StreamReader streamReader = tokenizer.StrmReader;

            char peekedChar = (char)streamReader.Peek();

            if (this.IsPossibleKeyword(peekedChar))
            {
                tokenizer.State = KeywordToken.Instance();
            }
            

        }

        /// <summary>
        /// Determines whether [is possible keyword] [the specified peeked char].
        /// </summary>
        /// <param name="peekedChar">The peeked char.</param>
        /// <returns>
        /// 	<c>true</c> if [is possible keyword] [the specified peeked char]; otherwise, <c>false</c>.
        /// </returns>
        private bool IsPossibleKeyword(char peekedChar)
        {
            bool retVal = false;

            //first letter of all keywords
            Match match = Regex.Match(peekedChar.ToString(), "[c|f|m|s|v|i|b|v|t|l|d|i|e|w|r]", RegexOptions.Compiled);

            retVal = match.Success;

            return retVal;
        }

        #endregion
    }
}
