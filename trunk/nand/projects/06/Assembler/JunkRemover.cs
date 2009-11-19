using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

using System.IO;

namespace Assembler
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
        /// <param name="inputStream">The input stream.</param>
        public JunkRemover(Stream inputStream)
        {
            reader = new StreamReader(inputStream);
        }

        /// <summary>
        /// Removes the junk from file.
        /// </summary>
        public byte[] RemoveJunk()
        {
            using (MemoryStream outputStream = new MemoryStream())
            using (StreamWriter writer = new StreamWriter(outputStream))
            {
                
                while (!reader.EndOfStream)
                {
                    string line = (reader.ReadLine());

                    // remove comments
                    Regex regex = new Regex(@"\/\/.*");
                    line = regex.Replace(line, "");

                    // ignore empty lines
                    if (!String.IsNullOrEmpty(line.Trim()))
                    {
                        writer.WriteLine(line);
                    }
                }
                writer.Flush();

                return outputStream.ToArray();
            }
            
        }


        #region IDisposable Members

        void IDisposable.Dispose()
        {
            reader.Dispose();
        }

        #endregion
    }
}
