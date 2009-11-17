using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.IO;

namespace Assembler
{
    /// <summary>
    /// Acts as the glue between the input file, 
    /// the junk, the parser and the codegenerator.
    /// </summary>
    public class Assembler
    {
        private string InputFilePath
        {
            get;
            set;
        }

        /// <summary>
        /// Dictionary type structure that holds all encountered symbols in the asm
        /// file and their line number +1 
        /// </summary>
        /// <value>The symbol table.</value>
        private SymbolTable SymbolTable
        {
            get;
            set;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Assembler"/> class.
        /// </summary>
        /// <param name="input">The input.</param>
        public Assembler(string input)
        {
            this.InputFilePath = input;
            this.SymbolTable = new SymbolTable();
        }

        /// <summary>
        /// Peforms the assemble of this inputFile.
        /// </summary>
        /// <returns></returns>
        public string Assemble()
        {
            // confusingly the binary represtation is actually a string of zeros and ones.
            string finalBinaryRepresentation = string.Empty;
            
            byte[] junkRemoved = this.RemoveJunk(this.InputFilePath);

            this.FillSymbolTable(junkRemoved);

            finalBinaryRepresentation = this.FinalPass(junkRemoved);

            return finalBinaryRepresentation;
        }

        /// <summary>
        /// Removes Junk From the stream and returns a byte array
        /// i.e. removes comments etc
        /// </summary>
        /// <returns></returns>
        private byte[] RemoveJunk(string inputFilePath)
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

        /// <summary>
        /// First pass of the assembly file.
        /// Finds all symbols and stores them in a Dictionary along (with the next line number)
        /// i.e. fills the symbol table
        /// </summary>
        /// <param name="inputBytes">The input bytes.</param>
        /// <param name="symbolTable">The symbol table.</param>
        private void FillSymbolTable(byte[] inputBytes)
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
                        // store the lable and next line
                        this.SymbolTable.AddEntry(parser.Symbol(), lineCounter);
                    }
                    else if (parser.CommandType() == Command.C_COMMAND
                        || parser.CommandType() == Command.A_COMMAND)
                    {
                        // ignore non label commands in the asm file.
                        lineCounter++;
                    }
                }
            }
        }

        /// <summary>
        /// Seconds the pass.
        /// TODO - break into smaller methods!!!
        /// after the first symbol table filling pass,
        /// we march thru the lines again, the parser gives us each command and its type
        /// we pass these to the code generator to build our binary representation
        /// </summary>
        /// <param name="inputBytes">The input bytes.</param>
        /// <param name="symbolTable">The symbol table.</param>
        /// <returns></returns>
        private string FinalPass(byte[] inputBytes)
        {
            MemoryStream inputStream = new MemoryStream(inputBytes);

            using (Parser parser = new Parser(inputStream))
            {
                StringBuilder fullBinaryListing = new StringBuilder();
                // magic number, new variables in the hack platform are store from address 16 upwards.
                int addressCounter = 16;

                //TODO  - horrible nested while and if chain - break into methods
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
                        if (this.SymbolTable.Contains(parser.Symbol()))
                        {
                            int address = this.SymbolTable.GetAddress(parser.Symbol());
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
                                this.SymbolTable.AddEntry(parser.Symbol(), addressCounter);
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
    }
}
