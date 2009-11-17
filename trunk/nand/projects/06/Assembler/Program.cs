using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.IO;

namespace Assembler
{
    public class Program
    {
        static void Main(string[] args)
        {
            if (args.Length == 2)
            {
                string inputPath = args[0];
                string outputPath = args[1];
                // TODO could do with some error handling around this , try catch invalid filepaths etc...
                Assembler assembler = new Assembler(inputPath);
                string assembledCode = assembler.Assemble();

                Program.WriteToFile(outputPath, assembledCode);
            }
            else
            {
                Console.WriteLine("Specify an input .asm file and an output file.");
            }
            
        }


        

        

       

        

        /// <summary>
        /// 
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="content"></param>
        private static void WriteToFile(string filePath, string content)
        {
            File.Delete(filePath);
            File.AppendAllText(filePath, content);
        }

    }
}
