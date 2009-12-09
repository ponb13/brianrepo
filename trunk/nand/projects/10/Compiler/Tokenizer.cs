using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Compiler
{
    public class Tokenizer : IDisposable
    {
        private IList<Pair<string, string>> tokens;
        private StreamReader streamReader;


        public Tokenizer(string filePath)
        {
            streamReader = new StreamReader(filePath);
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
            (char)this.streamReader.Read();
        }

        #region IDisposable Members

        public void Dispose()
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
