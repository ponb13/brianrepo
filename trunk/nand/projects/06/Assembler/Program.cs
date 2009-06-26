using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Assembler
{
    class Program
    {
        static void Main(string[] args)
        {
            string inputFilePath = args[0];

            using (FileStream fileStream = new FileStream(inputFilePath, FileMode.Open))
            using (MemoryStream outputStream = new MemoryStream())
            {
                JunkRemover junkRemover = new JunkRemover(fileStream, outputStream);

                using (Parser parser = new Parser(outputStream))
                {
                    while (parser.HasMoreCommands())
                    {
                        parser.Advance();
                        Console.WriteLine(parser.CommandType);
                    }
                    File.WriteAllBytes("C:/out2.txt", outputStream.ToArray());
                }
            }

            //while (parser.HasMoreCommands())
            //{ 
            //    parser.Advance();
            //    Console.WriteLine(parser.currentTxtCommand+" "+ parser.CommandType.ToString());
            //}
            

            Console.ReadKey();
        }

       
    }
}
