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

        /// <summary>
        /// Changes the state.
        /// </summary>
        /// <param name="tokenizer">The tokenizer.</param>
        private void ChangeState(ITokenizer tokenizer)
        {
            StreamReader streamReader = tokenizer.StrmReader;

            IState nextState = null;

            if (!streamReader.EndOfStream)
            {
                char peekedChar = (char)streamReader.Peek();

                //  the order of these ifs matters - 
                //  IsIntegerConstant must be checked before Identifier.
                if (this.IsPossibleKeyword(peekedChar))
                {
                    nextState = Keyword.Instance();
                }
                else if (this.IsWhiteSpace(peekedChar))
                {
                    tokenizer.StrmReader.Read();
                    nextState = NewToken.Instance();
                }
                else if (this.IsSymbol(peekedChar))
                {
                    nextState = Symbol.Instance();
                }
                else if (this.IsIntegerConstant(peekedChar))
                {
                    nextState = IntegerConstant.Instance();
                }
                else if (this.IsPossibleIdentifierCharacter(peekedChar))
                {
                    nextState = Identifier.Instance();
                }

                if (nextState != null)
                {
                    nextState.Read(tokenizer);
                }
            }
        }

        /// <summary>
        /// Determines if single char is valid as part of a Identifier
        /// alphanumeric and underscore are valid characters
        /// </summary>
        /// <param name="peekedChar"></param>
        /// <returns></returns>
        private bool IsPossibleIdentifierCharacter(char peekedChar)
        {
            string validIdentifierCharPattern = "[0-9a-zA-Z_]";
            Match match = Regex.Match(peekedChar.ToString(), validIdentifierCharPattern, RegexOptions.Compiled);

            return match.Success;
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
            Match match = Regex.Match(peekedStr, @"[.{}()[\]\|\,\;\+\-\*\/\&""\|\<\>\=\~]");
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
            //first letter of all keywords
            Match match = Regex.Match(peekedChar.ToString(), "[c|f|m|s|v|i|b|v|t|l|d|i|e|w|r]", RegexOptions.Compiled);

            return match.Success;
        }

        /// <summary>
        /// Determines whether [is integer constant] [the specified peeked char].
        /// </summary>
        /// <param name="peekedChar">The peeked char.</param>
        /// <returns>
        /// 	<c>true</c> if [is integer constant] [the specified peeked char]; otherwise, <c>false</c>.
        /// </returns>
        private bool IsIntegerConstant(char peekedChar)
        {
            Match match = Regex.Match(peekedChar.ToString(), "[0-9]", RegexOptions.Compiled);

            return match.Success;
        }

        #endregion
    }
}
