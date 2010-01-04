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
            // kind of pointless having a state for symbol...
            StreamReader streamReader = tokenizer.StrmReader;
            tokenizer.TokenCharacters.Append(streamReader.Read());

            this.ChangeState();
        }
        #endregion

        private void ChangeState(ITokenizer tokenizer)
        {
            tokenizer.State = TokenComplete.Instance();
        }
    }
}
