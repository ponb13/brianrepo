using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.IO;

namespace Assembler
{
    public class JunkRemover
    {
        private FileStream outputFileStream;
        
        public JunkRemover(string inputFilePath)
        {
            StreamReader sr = new StreamReader(inputFilePath);

            // open output file stream that we will write to
            outputFileStream = new FileStream(@"C:\temp.junk", FileMode.Create);

            outputFileStream.Write

           
            

        }
    }
}
