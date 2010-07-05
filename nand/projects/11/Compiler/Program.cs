using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml.Linq;
using System.Diagnostics;

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
                    string inputPath = args[0];
                    foreach (string filepath in Directory.GetFiles(inputPath, @"*.jack"))
                    {
                        Compiler compiler = new Compiler(filepath);
                        compiler.Compile();
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
    }
}
