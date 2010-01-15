using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Interfaces;
using System.Text.RegularExpressions;

namespace States
{
    public class IntegerConstant : IState
    {
        #region singleton logic
        private static IState state = new IntegerConstant();

        public static IState Instance()
        {
            tokenCharacters = new StringBuilder();
            return state;
        }
        #endregion

        #region IState Members
        private static StringBuilder tokenCharacters;
        public StringBuilder TokenCharacters
        {
            get { return IntegerConstant.tokenCharacters; }
            set { IntegerConstant.tokenCharacters = value; }
        }

        public void Read(ITokenizer tokenizer)
        {
            while (!tokenizer.StrmReader.EndOfStream && IsValidIntegerChar((char)tokenizer.StrmReader.Peek()))
            {
                this.TokenCharacters.Append((char)tokenizer.StrmReader.Read());
            }

            tokenizer.Tokens.Add(this.CreateTokenObject());
            this.ChangeState(tokenizer);
        }

        private void ChangeState(ITokenizer tokenizer)
        {
            IState nextState = NewToken.Instance();
            nextState.Read(tokenizer);
        }

        /// <summary>
        /// Determines whether [is valid integer] [the specified peeked char].
        /// </summary>
        /// <param name="peekedChar">The peeked char.</param>
        /// <returns>
        /// 	<c>true</c> if [is valid integer] [the specified peeked char]; otherwise, <c>false</c>.
        /// </returns>
        private bool IsValidIntegerChar(char peekedChar)
        {
            Match match = Regex.Match(peekedChar.ToString(), "[0-9]");
            return match.Success;
        }

        #endregion
    }
}
