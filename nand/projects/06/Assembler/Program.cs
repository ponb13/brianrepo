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

                // Problem with streams - you can't reuse them so need to create a new memory stream
                MemoryStream outputStreamCopy = new MemoryStream(outputStream.ToArray());

                using (Parser parser = new Parser(outputStreamCopy))
                {
                    while (parser.HasMoreCommands())
                    {
                        parser.Advance();
                        
                        if (parser.CommandType() == Command.C_COMMAND)
                        {
                            Console.WriteLine(parser.Dest());
                        }
                    }
                }
            }
            Console.ReadKey();
        }

       
    }
}
