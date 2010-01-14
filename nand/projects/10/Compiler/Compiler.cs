using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
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
            try
            {
                using (Tokenizer tokenizer = new Tokenizer(this.inputPath))
                {
                    IList<Pair<string, string>> tokens = tokenizer.GetTokens();
                    TokensToXml(tokens);
                }
            }
            catch (Exception ex)
            {
                // TODO remove exception handling from here
                Console.WriteLine(ex);
                Console.ReadKey();
            }
        }

        private void TokensToXml(IList<Pair<string, string>> tokens)
        {
            using(StreamWriter streamWriter = new StreamWriter(File.Create(@"..\..\..\TestOutputFiles\ArrayTest\t.txt")))
            {
                streamWriter.Write("<tokens>");
                streamWriter.Write(Environment.NewLine);
                foreach (Pair<string, string> tokenPair in tokens)
                {
                    if (tokenPair.Value1 != "Comment")
                    {
                        streamWriter.Write("<" + tokenPair.Value1.ToLower() + ">");
                        streamWriter.Write(" "+tokenPair.Value2+" ");
                        streamWriter.Write("</" + tokenPair.Value1.ToLower() + ">");
                        streamWriter.Write(Environment.NewLine);
                    }
                }

                streamWriter.Write("</tokens>");


            }
        }

       
    }
}
