using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

using System.IO;

namespace Assembler
{
    public class JunkRemover
    {

        private StreamReader reader;
        private StreamWriter writer;
        
        public JunkRemover(Stream inputStream, Stream outputStream)
        {
            reader = new StreamReader(inputStream);
            writer = new StreamWriter(outputStream);
            RemoveJunk();
            
        }

        private void RemoveJunk()
        {
            while (!reader.EndOfStream)
            {
                string line = (reader.ReadLine());
                
                // remove comments
                Regex regex = new Regex(@"\/\/.*");
                line = regex.Replace(line, "");

                // ignore empty lines
                if (!String.IsNullOrEmpty(line))
                {
                    writer.WriteLine(line);
                }
            }

            writer.Flush();
        }

    }
}
