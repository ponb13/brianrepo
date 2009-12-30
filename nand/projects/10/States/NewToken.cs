using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Interfaces;

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

        void IState.ReadChar(ITokenizer tokenizer)
        {
            // we're in a new token so clear the previously read chars
            tokenizer.TokenChars.Clear();
        }

        #endregion
    }
}
