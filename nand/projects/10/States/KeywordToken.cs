using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Interfaces;

namespace States
{
    public class KeywordToken : IState
    {
        #region singleton logic
        private static IState state = new KeywordToken();

        private KeywordToken()
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
            //take next 12 chars (longest keyword + space or symbol)
            char[] buffer = new char[12];
            //tokenizer.StrmReader.
        }

        #endregion
    }
}
