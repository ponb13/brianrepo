using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Interfaces;
using States;

namespace Compiler
{
    public class Tokenizer : ITokenizer, IDisposable
    {
        
        private StreamReader streamReader;

        public Tokenizer(string filePath)
        {
            streamReader = new StreamReader(filePath);
            this.State = NewToken.Instance();
        }

        /// <summary>
        /// Determines whether the file [has more tokens].
        /// </summary>
        /// <returns>
        /// 	<c>true</c> if [has more tokens]; otherwise, <c>false</c>.
        /// </returns>
        public bool HasMoreTokens()
        {
            return !this.streamReader.EndOfStream;
        }

        public Pair<string, string> GetNextToken()
        {
            this.State = NewToken.Instance();

            while(this.State != typeof(TokenComplete))
            {
                this.State.ReadChar(this);
            }

            return null;
        }

        #region IDisposable Members

        public void Dispose()
        {
            throw new NotImplementedException();
        }

        #endregion

        #region ITokenizer Members

        /// <summary>
        /// Gets or sets the state.
        /// </summary>
        /// <value>The state.</value>
        public IState State
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the token chars.
        /// </summary>
        /// <value>The token chars.</value>
        public IList<char> TokenChars
        {
            get;
            set;
        }

        #endregion
    }
}
