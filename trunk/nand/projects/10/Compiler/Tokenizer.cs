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

        public IList<Pair<string, string>> Tokens
        {
            get;
            set;
        }

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
            this.Tokens = new List<Pair<string,string>>();
            this.StrmReader = new StreamReader(filePath);
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

        public IList<Pair<string, string>> GetTokens()
        {
            this.State = NewToken.Instance();

           
            this.State.Read(this);
            

            return this.Tokens;
        }

        
        #region IDisposable Members

        public void Dispose()
        {
            this.StrmReader.Dispose();
        }

        #endregion

    }
}
