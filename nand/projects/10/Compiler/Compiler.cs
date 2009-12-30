using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Compiler
{
    public class Compiler
    {
        private string inputPath;
        
        public Compiler(string inputPath)
        {
            this.inputPath = inputPath;

            //this.tokenizerOutputPath = Path.Combine(Path.GetDirectoryName(inputPath), @"Output\" + Path.GetFileNameWithoutExtension(inputPath) + "T.xml");
        }

        public void Compile()
        {
            Tokenizer tokenizer = new Tokenizer(this.inputPath);
        }

       
    }
}
