using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace VMTranslator
{
    public class Program
    {
        static void Main(string[] args)
        {
            if (args.Count() == 2)
            {
                string inputPath = args[0];
                string outputPath = args[1];

                Translator translator = new Translator(inputPath, outputPath);
                translator.Translate();

                Console.WriteLine("Translation Complete, written to: " + Path.GetFullPath(outputPath));
                Console.ReadKey();
            }
            else
            {
                Console.WriteLine("Please supplier input file / directory and output file path.");
            }

        }
    }
}
