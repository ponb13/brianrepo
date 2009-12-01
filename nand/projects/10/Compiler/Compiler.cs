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
        private string outputPath;
        private string currentFileContents; 
        
        public Compiler(string inputPath, string outputPath)
        {
            this.inputPath = inputPath;
            this.outputPath = outputPath;
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
