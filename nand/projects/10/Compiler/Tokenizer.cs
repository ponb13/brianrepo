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

        private Stack<Pair<string, string>> previousTokens = new Stack<Pair<string, string>>();

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

        /// <summary>
        /// Gets or sets the previous token.
        /// Cache the previous token, useful for making decisions on state change.
        /// </summary>
        /// <value>The previous token.</value>
        public Stack<Pair<string, string>> PreviousTokens
        {
            get
            {
                return this.previousTokens;
            }
            set
            {
                this.previousTokens = value;
            }
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

            Pair<string, string> token = this.CreateTokenObject(previousState);

            //Cache the token
            this.PreviousTokens.Push(token);

            return token;
        }

        /// <summary>
        /// Creates the token object.
        /// </summary>
        /// <param name="previousState">previous state i.e. the state before token complete is the token name!</param>
        /// <returns></returns>
        private Pair<string,string> CreateTokenObject(IState previousState)
        {
            string stateName = previousState.ToString().Split('.').Last();
            
            return new Pair<string, string>
            {
                Value1 = stateName,
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
