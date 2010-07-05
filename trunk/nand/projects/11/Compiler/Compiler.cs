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

            using (VmWriter vmWriter = new VmWriter(GetOutputPath()))
            {
                CompilationEngineVm compilationEngineVm = new CompilationEngineVm(tokens, vmWriter);
                compilationEngineVm.CompileClass();
            }
        }

        string GetOutputPath()
        {
            string outputfileName =  Path.GetFileNameWithoutExtension(this.inputPath) + ".vm";
            return this.inputPath = inputPath.Replace(Path.GetFileName(inputPath), outputfileName);
        }

    }
}
