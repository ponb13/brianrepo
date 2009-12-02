using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

using System.IO;

namespace Compiler
{
    /// <summary>
    /// Very Simple Junk remover
    /// Removes comments and empty lines.
    /// </summary>
    public class JunkRemover : IDisposable
    {
        private StreamReader reader;

        /// <summary>
        /// Initializes a new instance of the <see cref="JunkRemover"/> class.
        /// </summary>
        /// <param name="path">The path.</param>
        public JunkRemover(string path)
        {
            reader = new StreamReader(path);
        }

        /// <summary>
        /// Returns the contents of the file as a string with junk removed
        /// </summary>
        public string RemoveJunk()
        {
            string fullFileContents = this.reader.ReadToEnd();

            fullFileContents = Regex.Replace(fullFileContents, @"/\*[\d\D]*?\*/|\/\/.*","");

            return fullFileContents;
        }


        #region IDisposable Members

        void IDisposable.Dispose()
        {
            reader.Dispose();
        }

        #endregion
    }
}
