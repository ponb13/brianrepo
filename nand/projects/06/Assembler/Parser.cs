using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.IO;

namespace Assembler
{
    public class Parser
    {

        private Stream inputStream;

        public Parser(Stream inputFileStream)
        {
            this.inputStream = inputFileStream;
        }
    }
}
