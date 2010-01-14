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
            string inputPath = @"../../../TestFiles/ArrayTest/Main.jack";
            //string inputPath = @"../../../TestFiles/UnitTestFiles/Junk.jack";

            Compiler compiler = new Compiler(inputPath);
            compiler.Compile();
        }
    }
}
