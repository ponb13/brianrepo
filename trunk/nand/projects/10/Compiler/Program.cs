using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Compiler
{
    public class Program
    {
        static void Main(string[] args)
        {
            try
            {
                //string inputPath = @"../../../TestFiles/ExpressionlessSquare/";
                string inputPath = @"../../../TestFiles/UnitTestFiles/";

                foreach (string filepath in Directory.GetFiles(inputPath, @"*.jack"))
                {
                    Compiler compiler = new Compiler(filepath);
                    compiler.Compile();
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
