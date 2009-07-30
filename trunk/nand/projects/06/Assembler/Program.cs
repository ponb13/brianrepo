using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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
            // fill sysmbol table in first pass
            Program.FirstPass(junkRemoved, symbolTable);

            //replace all label
            string finalBinaryOutput = Program.SecondPass(junkRemoved,symbolTable);

            Program.WriteToFile(@"C:\Documents and Settings\brian\My Documents\dev\brianrepo\nand\projects\06\TestFiles\BRIANPongWithSymbols.asm", finalBinaryOutput);
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
        private static void FirstPass(byte[] inputBytes, SymbolTable symbolTable)
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
                        symbolTable.AddEntry(parser.Symbol(), lineCounter);
                    }
                    else if (parser.CommandType() == Command.C_COMMAND
                        || parser.CommandType() == Command.A_COMMAND)
                    {
                        lineCounter++;
                    }
                }
            }
        }

        private static string SecondPass(byte[] inputBytes, SymbolTable symbolTable)
        {
            // Problem with streams - you can't reuse them so need to create a new memory stream
            MemoryStream inputStream = new MemoryStream(inputBytes);

            using (Parser parser = new Parser(inputStream))
            {
                StringBuilder fullBinaryListing = new StringBuilder();
                int addressCounter = 16;

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
                        if (symbolTable.Contains(parser.Symbol()))
                        {
                            int address = symbolTable.GetAddress(parser.Symbol());
                            binaryLine = CodeGenerator.Get_AInstruction(address.ToString());
                        }
                        else
                        {
                           
                            int literalVal;
                            //check if its int literal if it is just add it
                            if (int.TryParse(parser.Symbol(), out literalVal))
                            {
                                binaryLine = CodeGenerator.Get_AInstruction(literalVal.ToString());
                            }
                            else
                            {

                                // add the new address
                                symbolTable.AddEntry(parser.Symbol(), addressCounter);
                                binaryLine = CodeGenerator.Get_AInstruction(addressCounter.ToString());
                                addressCounter++;
                            }
                        }
                        
                        fullBinaryListing.Append(binaryLine + Environment.NewLine);
                    }
                }

                return fullBinaryListing.ToString();
            }
        }

        // replace old value with new
        private static string ReplaceSymbol(string codeLine, SymbolTable symbolTable, string symbolToRelplace)
        {
            string result = null;
            if (symbolTable.Contains(symbolToRelplace))
            {
                int intVal = symbolTable.GetAddress(symbolToRelplace);
                result = codeLine.Replace(symbolToRelplace, intVal.ToString());
            }
            return result;
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
