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
            else
            {
                tokenizer.State = NewToken.Instance();
            }

            tokenizer.State.Read(tokenizer);
        }
    }
}
