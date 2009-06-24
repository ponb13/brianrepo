using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assembler
{
    class Program
    {
        static void Main(string[] args)
        {
            string inputFilePath = args[0];

            using (Parser parser = new Parser(inputFilePath))
            {
                while (parser.HasMoreCommands())
                { 
                    parser.Advance();
                    Console.WriteLine(parser.CurrentCommand);
                }
            }
        }
    }
}
