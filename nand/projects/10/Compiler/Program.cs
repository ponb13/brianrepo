using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Compiler
{
    public class Program
    {
        static void Main(string[] args)
        {
            Compiler compiler = new Compiler(@"C:\Documents and Settings\brian\My Documents\code\brianrepo\nand\projects\10\TestFiles\ArrayTest\Main.jack", @"C:\asd.txt");
            compiler.Compile();
        }
    }
}
