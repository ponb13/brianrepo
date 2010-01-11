using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Interfaces;
using System.IO;

namespace States
{
    public class Symbol : IState
    {
        #region singleton logic
        private static IState state = new Symbol();
        private static StringBuilder tokenCharacters;

        private Symbol()
        {

        }

        public static IState Instance()
        {
            // always clear chars when instance is called
            Symbol.tokenCharacters = new StringBuilder();
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
            get { return Symbol.tokenCharacters; }
            set { Symbol.tokenCharacters = value; }
        }

        public void Read(ITokenizer tokenizer)
        {
            StreamReader streamReader = tokenizer.StrmReader;
            char symbolChar = (char)streamReader.Read();
            this.TokenCharacters.Append(symbolChar);

            tokenizer.Tokens.Add(this.CreateTokenObject());

            this.ChangeState(tokenizer, symbolChar);
        }
        #endregion

        /// <summary>
        /// Changes the state of the tokenizer
        /// </summary>
        /// <param name="tokenizer">The tokenizer.</param>
        /// <param name="symbolCharacter">The symbol character.</param>
        private void ChangeState(ITokenizer tokenizer, char symbolChar)
        {
            this.CreateTokenObject();

            if (symbolChar == '"')
            {
                tokenizer.State = StringConstant.Instance();
            }
            else if(this.IsComment( tokenizer.StrmReader, symbolChar))
            {
                tokenizer.State = Comment.Instance();
            }
            else
            {
                tokenizer.State = NewToken.Instance();
            }

            tokenizer.State.Read(tokenizer);
        }

        /// <summary>
        /// Checks if the current is the beginning of a comment
        /// </summary>
        /// <param name="streamReader"></param>
        /// <param name="symbol"></param>
        private bool IsComment(StreamReader streamReader, char currentChar)
        {
            bool retVal = false;
            char nextChar = (char)streamReader.Peek();

            // matches '//' or '/*'
            if (currentChar == '/')
            {
                if (nextChar == '/' || nextChar == '*')
                {
                    retVal = true;
                }
            }

            return retVal;
        }
    }
}
