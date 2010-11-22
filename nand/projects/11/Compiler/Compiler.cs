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
        private string outputDirectory;

        public Compiler(string inputPath, string outputDirectory)
        {
            this.inputPath = inputPath;
            this.outputDirectory = outputDirectory;

            if (!String.IsNullOrWhiteSpace(outputDirectory))
            {
                if (!Directory.Exists(outputDirectory))
                {
                    Directory.CreateDirectory(outputDirectory);
                }
            }
        }

        public void Compile()
        {
            IList<Pair<string, string>> tokens = null;
            
            using (Tokenizer tokenizer = new Tokenizer(this.inputPath))
            {
                tokens = tokenizer.GetTokens();
            }

            using (VmWriter vmWriter = new VmWriter(GetOutputPath()))
            {
                CompilationEngineVm compilationEngineVm = new CompilationEngineVm(tokens, vmWriter, Path.GetFileNameWithoutExtension(inputPath));
                compilationEngineVm.CompileClass();
            }
        }

        string GetOutputPath()
        {
            // write to different directory if supplied
            string outputfileName = Path.GetFileNameWithoutExtension(this.inputPath) + ".vm";
            if (string.IsNullOrEmpty(outputDirectory))
            {
                
                return this.inputPath = inputPath.Replace(Path.GetFileName(inputPath), outputfileName);
            }
            else
            {
                return Path.Combine(outputDirectory, outputfileName);
            }
            
        }

    }
}
