using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml.Linq;


namespace Compiler
{
    public class Program
    {
        /// <summary>
        /// Outputs parse tree as xml... for now...
        /// </summary>
        /// <param name="args"></param>
        static void Main(string[] args)
        {
            try
            {
                if (args != null && args.Length > 0)
                {
                    string inputPath = args[1];
                    foreach (string filepath in Directory.GetFiles(inputPath, @"*.jack"))
                    {
                        string outputPath = Program.GetOutputFilePath(inputPath, filepath);
                        Compiler compiler = new Compiler(filepath);
                        Program.WriteOuput(compiler.Compile(), outputPath);
                    }
                }
                else
                {
                    Console.WriteLine("Please specify a directory or file path.");
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                Console.ReadKey();
            }

        }

        private static string GetOutputFilePath(string inputPath, string filepath)
        {
            string outputDir = inputPath + @"/Output";

            if(!Directory.Exists(outputDir))
            {
                Directory.CreateDirectory(outputDir);
            }

            return outputDir + @"/" + Path.GetFileNameWithoutExtension(filepath) + ".xml";
        }

        private static void WriteOuput(XElement compiledClass, string outputPath)
        {
            if (File.Exists(outputPath))
            {
                File.Delete(outputPath);
            }
            using (StreamWriter sw = new StreamWriter(File.Create(outputPath)))
            {
                sw.Write(compiledClass.ToString());
            }
        }

    }
}
