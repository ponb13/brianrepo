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
        private IList<Pair<string, string>> tokens = new List<Pair<string, string>>();
        
        public Compiler(string inputPath)
        {
            this.inputPath = inputPath;
        }

        public void Compile()
        {
            using (Tokenizer tokenizer = new Tokenizer(this.inputPath))
            {
                while (tokenizer.HasMoreTokens())
                {
                    Pair<string, string> token = tokenizer.GetNextToken();
                    tokens.Add(token);
                }
            }
        }

       
    }
}
