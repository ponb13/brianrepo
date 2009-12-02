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
            string inputPath = @"C:\Documents and Settings\brian\My Documents\code\brianrepo\nand\projects\10\TestFiles\ArrayTest\Main.jack";
            

            Compiler compiler = new Compiler(inputPath);
            compiler.Compile();
        }
    }
}
