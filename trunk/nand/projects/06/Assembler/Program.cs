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

            
            
            byte[] junkRemoved = Program.RemoveJunk(inputFilePath);

            SymbolTable symbolTable = new SymbolTable();
            Program.FirstPass(junkRemoved, symbolTable);

            

            string finalBinaryOutput = Program.ThirdPass(junkRemoved);

            Program.WriteToFile(@"C:\BriPong.hack", finalBinaryOutput);
        }

        /// <summary>
        /// Removes Junk From the stream and returns a byte array
        /// </summary>
        /// <returns></returns>
        private static byte[] RemoveJunk(string inputFilePath)
        {
            byte[] result;

            using (FileStream fileStream = new FileStream(inputFilePath, FileMode.Open))
            using (MemoryStream outputStream = new MemoryStream())
            {
                JunkRemover junkRemover = new JunkRemover(fileStream, outputStream);

                result = outputStream.ToArray();
            }

            return result;
        }

        // build the symbol table
        private static SymbolTable FirstPass(byte[] inputBytes, SymbolTable symbolTable)
        {
            using (MemoryStream inputStream = new MemoryStream(inputBytes))
            using (Parser parser = new Parser(inputStream))
            {
                int lineCounter = 0;
                while (parser.HasMoreCommands())
                {
                    parser.Advance();
                    if (parser.CommandType() == Command.L_COMMAND)
                    {
                        // store the lable and next line, check that this should be +1
                        symbolTable.AddEntry(parser.Symbol(), lineCounter + 1);
                    }
                    else if (parser.CommandType() == Command.C_COMMAND
                        || parser.CommandType() == Command.A_COMMAND)
                    {
                        lineCounter++;
                    }
                }
            }

            // explicitly return
            return symbolTable;
        }

        private static byte[] SecondPass(byte[] inputBytes, SymbolTable symbolTable)
        {
            using (MemoryStream inputStream = new MemoryStream(inputBytes))
            using (Parser parser = new Parser(inputStream))
            {
                while (parser.HasMoreCommands())
                {
                    parser.Advance();
                    // todo 
                    
                }
            }
        }

        private static string ThirdPass(byte[] inputBytes)
        {
            // Problem with streams - you can't reuse them so need to create a new memory stream
            MemoryStream inputStream = new MemoryStream(inputBytes);

            using (Parser parser = new Parser(inputStream))
            {
                StringBuilder fullBinaryListing = new StringBuilder();

                while (parser.HasMoreCommands())
                {
                    String binaryLine = String.Empty;
                    parser.Advance();
                    if (parser.CommandType() == Command.C_COMMAND)
                    {
                        binaryLine = CodeGenerator.GetFullCInstruction(parser.Comp(), parser.Dest(), parser.Jump()) + Environment.NewLine;
                        fullBinaryListing.Append(binaryLine);
                    }
                    else if (parser.CommandType() == Command.A_COMMAND)
                    {
                        binaryLine = CodeGenerator.Get_AInstruction(parser.Symbol());
                        fullBinaryListing.Append(binaryLine + Environment.NewLine);
                    }
                }

                return fullBinaryListing.ToString();
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
