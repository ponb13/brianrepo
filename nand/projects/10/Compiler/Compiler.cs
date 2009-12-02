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
        private string tokenizerOutputPath;
        private string currentFileContents; 
        
        public Compiler(string inputPath)
        {
            this.inputPath = inputPath;

            this.tokenizerOutputPath = Path.Combine(Path.GetDirectoryName(inputPath), @"Output\" + Path.GetFileNameWithoutExtension(inputPath) + "T.xml");
        }

        public void Compile()
        {
            this.RemoveJunk();
            File.AppendAllText(outputPath, currentFileContents); 
        }

        /// <summary>
        /// Removes the junk from file.
        /// </summary>
        private void  RemoveJunk()
        {
            using(JunkRemover junkRemover = new JunkRemover(this.inputPath))
            {
                this.currentFileContents = junkRemover.RemoveJunk();
            }
        }
    }
}
