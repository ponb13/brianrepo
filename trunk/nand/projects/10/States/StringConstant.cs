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

        private StringConstant()
        {
        }

        public static IState Instance()
        {
            return state;
        }
        #endregion

        #region IState Members

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
                tokenizer.TokenCharacters.Append((char)streamReader.Read());
            }

            this.ChangeState(tokenizer);
        }

        private void ChangeState(ITokenizer tokenizer)
        {
            tokenizer.State = TokenComplete.Instance();
        }

        #endregion
    }
}
