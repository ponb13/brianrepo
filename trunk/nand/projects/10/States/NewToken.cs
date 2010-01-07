﻿using System;
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
                tokenizer.State = Keyword.Instance();
            }
            else if(this.IsWhiteSpace(peekedChar))
            {
                //  Just consume white space, don't change change state
                tokenizer.StrmReader.Read();
            }
            else if (this.IsSymbol(peekedChar))
            {
                tokenizer.State = Symbol.Instance();
            }
            else if (this.IsStringConstant(tokenizer))
            {
                tokenizer.PreviousTokens.Pop();
                
                tokenizer.State = StringConstant.Instance();
            }

        }

        /// <summary>
        /// Determines whether [is string constant].
        /// Check previous tokens to determine if this is a string constant
        /// i.e if the previous token was quotes (") and they aren't closing quotes
        /// then we have a string constant.
        /// </summary>
        /// <returns>
        /// 	<c>true</c> if [is string constant]; otherwise, <c>false</c>.
        /// </returns>
        private bool IsStringConstant(ITokenizer tokenizer)
        {
            bool retVal = false;

            if (tokenizer.PreviousTokens.Count > 0)
            {
                Pair<string, string> previousToken = tokenizer.PreviousTokens.Pop();

                if (previousToken.Value1 == "\"" && previousToken.Value2 == "Symbol")
                {
                    Pair<string, string> nextPreviousToken = tokenizer.PreviousTokens.Pop();
                    if (nextPreviousToken.Value1 == "\"" && nextPreviousToken.Value2 == "Symbol"
                        || nextPreviousToken.Value2 == "StringConstant")
                    {
                        retVal = false;
                    }
                    else
                    {
                        retVal = true;
                    }
                }
            }

            return retVal;
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
