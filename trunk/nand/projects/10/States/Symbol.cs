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

        private Symbol()
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
            StreamReader streamReader = tokenizer.StrmReader;
            char symbolChar = (char)streamReader.Read();
            tokenizer.TokenCharacters.Append(symbolChar);

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
            tokenizer.State = TokenComplete.Instance();
        }
    }
}
