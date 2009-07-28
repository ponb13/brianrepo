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
                MemoryStream junkRemovedStream = new MemoryStream(outputStream.ToArray());

                using (Parser parser = new Parser(junkRemovedStream))
                {
                    StringBuilder fullBinaryListing = new StringBuilder();

                    while (parser.HasMoreCommands())
                    {
                       String binaryLine = String.Empty;
                       parser.Advance();
                       if (parser.CommandType() == Command.C_COMMAND)
                       {
                           binaryLine = CodeGenerator.GetFullCInstruction(parser.Comp(),parser.Dest(), parser.Jump())+ Environment.NewLine;
                           fullBinaryListing.Append(binaryLine);
                       }
                       else if (parser.CommandType() == Command.A_COMMAND)
                       {
                           binaryLine = CodeGenerator.Get_AInstruction(parser.Symbol());
                           fullBinaryListing.Append(binaryLine + Environment.NewLine);
                       }
                    }

                    Program.WriteToFile(@"C:\BriPong.hack", fullBinaryListing.ToString());
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="content"></param>
        private static void WriteToFile(string filePath, string content)
        {
            File.Delete(filePath);
            File.AppendAllText(filePath, content);
        }
       
    }
}
