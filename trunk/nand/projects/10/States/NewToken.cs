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
        private static StringBuilder tokenCharacters = new StringBuilder();

        private NewToken()
        {
        }

        public static IState Instance()
        {
            // always clear chars when instance is called
            state.TokenCharacters = new StringBuilder();
            return state;
        }
        #endregion
        
        #region IState Members
        /// <summary>
        /// Gets or sets the token characters.
        /// </summary>
        /// <value>The token characters.</value>
        public StringBuilder TokenCharacters
        {
            get { return NewToken.tokenCharacters; }
            set { NewToken.tokenCharacters = value; }
        }

        public void Read(ITokenizer tokenizer)
        {
            this.ChangeState(tokenizer);
        }

        private void ChangeState(ITokenizer tokenizer)
        {
            StreamReader streamReader = tokenizer.StrmReader;

            if (!streamReader.EndOfStream)
            {

                char peekedChar = (char)streamReader.Peek();

                if (this.IsPossibleKeyword(peekedChar))
                {
                    tokenizer.State = Keyword.Instance();
                }
                else if (this.IsWhiteSpace(peekedChar))
                {
                    //  Just consume white space, don't change change state
                    tokenizer.StrmReader.Read();
                }
                else if (this.IsSymbol(peekedChar))
                {
                    tokenizer.State = Symbol.Instance();
                }
            

                tokenizer.State.Read(tokenizer);
            }
        }

        /// <summary>
        /// Determines whether the specified peeked char is symbol.
        /// </summary>
        /// <param name="peekedChar">The peeked char.</param>
        /// <returns>
        /// 	<c>true</c> if the specified peeked char is symbol; otherwise, <c>false</c>.
        /// </returns>
        private bool IsSymbol(char peekedChar)
        {
            string peekedStr = peekedChar.ToString();
            Match match = Regex.Match(peekedStr,@"[{}()[\]\|\,\;\+\-\*\/\&""\|\<\>\=\~]");
            return match.Success;
        }

        /// <summary>
        /// Determines whether [is white space] [the specified peeked char].
        /// </summary>
        /// <param name="peekedChar">The peeked char.</param>
        /// <returns>
        /// 	<c>true</c> if [is white space] [the specified peeked char]; otherwise, <c>false</c>.
        /// </returns>
        private bool IsWhiteSpace(char peekedChar)
        {
            string peekedStr = peekedChar.ToString();
            Match match = Regex.Match(peekedChar.ToString(), @"\s", RegexOptions.Compiled);
            return match.Success;
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
