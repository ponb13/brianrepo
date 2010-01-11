using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Interfaces;
using System.IO;
using System.Text.RegularExpressions;

namespace States
{
    public class StringConstant : IState
    {
        #region singleton logic
        private static IState state = new StringConstant();
        private static StringBuilder tokenCharacters;

        private StringConstant()
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
            get { return StringConstant.tokenCharacters; }
            set { StringConstant.tokenCharacters = value; }
        }

        /// <summary>
        /// Reads for the tokenizers stream.
        /// Reads all chars between double quotes
        /// Changes state to TokenComplete when finished
        /// </summary>
        /// <param name="tokenizer">The tokenizer.</param>
        public void Read(ITokenizer tokenizer)
        {
            StreamReader streamReader = tokenizer.StrmReader;

            // Symbol state has already read the first quote
            // read untill we hit closing quotes
            while ((char)streamReader.Peek() != '"')
            {
                if (streamReader.EndOfStream)
                {
                    // TODO: Clean this up - proper exception line number etc
                    throw new Exception("End of stream before closing quotes found");
                }

                this.TokenCharacters.Append((char)streamReader.Read());
            }

            tokenizer.Tokens.Add(this.CreateTokenObject());


            this.ChangeState(tokenizer);
        }

        private void ChangeState(ITokenizer tokenizer)
        {

            // HACK - we know that the peeked char will be a closing "
            // so create a new token and add it to list
            IState symbolState = Symbol.Instance();
            symbolState.TokenCharacters.Append((char)tokenizer.StrmReader.Read());
            tokenizer.Tokens.Add(symbolState.CreateTokenObject());

            tokenizer.State = NewToken.Instance();
            tokenizer.State.Read(tokenizer);
        }

        #endregion
    }
}
