using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Interfaces;

namespace States
{
    public class TokenComplete : IState
    {
        #region singleton logic
        private static IState state = new TokenComplete();

        private TokenComplete()
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
            // do nothing
        }

        #endregion
    }
}
