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
        public StringBuilder TokenCharacters
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the stream reader.
        /// public so that state classes can access stream
        /// </summary>
        /// <value>The stream reader.</value>
        public StreamReader StrmReader
        {
            get;
            set;
        }

        #endregion
        
        public Tokenizer(string filePath)
        {
            this.StrmReader = new StreamReader(filePath);
            this.TokenCharacters = new StringBuilder();
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
            return !this.StrmReader.EndOfStream;
        }

        public Pair<string, string> GetNextToken()
        {
            this.State = NewToken.Instance();

            IState previousState = NewToken.Instance();

            while (this.State.GetType() != typeof(TokenComplete))
            {
                // save previous state, when parse to end of token
                // we'd only get TokenComplete state at the end otherwise
                previousState = this.State;
                this.State.Read(this);
            }

            return new Pair<string, string>
            {
                Value1 = previousState.ToString(),
                Value2 = this.TokenCharacters.ToString()
            };
        }

        #region IDisposable Members

        public void Dispose()
        {
            this.StrmReader.Dispose();
        }

        #endregion

    }
}
