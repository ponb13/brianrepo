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
                TokensToXml(tokens);
            }

            CompilationEngine compilationEngine = new CompilationEngine(tokens);
            XElement xml = compilationEngine.CompileClass();
            Console.WriteLine(xml.ToString());
        }

        private void TokensToXml(IList<Pair<string, string>> tokens)
        {
            using (StreamWriter streamWriter = new StreamWriter(File.Create("..\\..\\..\\TestOutputFiles\\ArrayTest\\" + Path.GetFileNameWithoutExtension(this.inputPath) + ".xml")))
            {
                streamWriter.Write("<tokens>");
                streamWriter.Write(Environment.NewLine);
                foreach (Pair<string, string> tokenPair in tokens)
                {
                    if (tokenPair.Value1 != "Comment")
                    {
                        streamWriter.Write("<" + tokenPair.Value1.ToLower() + ">");
                        streamWriter.Write(" " + tokenPair.Value2 + " ");
                        streamWriter.Write("</" + tokenPair.Value1.ToLower() + ">");
                        streamWriter.Write(Environment.NewLine);
                    }
                }

                streamWriter.Write("</tokens>");


            }
        }


    }
}
