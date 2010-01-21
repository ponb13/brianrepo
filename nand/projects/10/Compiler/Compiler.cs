using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml.Linq;
using Interfaces;

namespace Compiler
{
    public class Compiler
    {
        private string inputPath;

        public Compiler(string inputPath)
        {
            this.inputPath = inputPath;
        }

        public void Compile()
        {
            IList<Pair<string, string>> tokens = null;
            
            using (Tokenizer tokenizer = new Tokenizer(this.inputPath))
            {
                tokens = tokenizer.GetTokens();
            }

            tokens=  tokens.Where(t => t.Value1 != "Comment").ToList();

            CompilationEngine compilationEngine = new CompilationEngine(tokens);
            XElement xml = compilationEngine.CompileClass();
            Console.WriteLine(xml.ToString());
        }

    }
}
