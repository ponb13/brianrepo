﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace VM
{
    class Program
    {
        static void Main(string[] args)
        {
            //string filePath = @"..\..\..\StackArithmetic\SimpleAdd\SimpleAdd.vm";
            //string outPutFilePath = @"..\..\..\StackArithmetic\SimpleAdd\SimpleAdd.asm";

            string filePath = @"..\..\..\MemoryAccess\BasicTest\BasicTest.vm";
            string outPutFilePath = @"..\..\..\MemoryAccess\BasicTest\BasicTest.asm";

            using (Parser parser = new Parser(filePath))
            using (CodeWriter codeWriter = new CodeWriter(outPutFilePath))
            {
                // TODO code writer need SetFilePath set
                while (parser.HasMoreCommands())
                {
                    parser.Advance();
                    CommandType commandType = parser.GetCommandType();

                    if (commandType == CommandType.C_PUSH)
                    {
                        codeWriter.WritePushPop(commandType, parser.GetArg1(), int.Parse(parser.GetArg2()));
                    }
                    if (commandType == CommandType.C_POP)
                    {
                        codeWriter.WritePushPop(commandType, parser.GetArg1(), int.Parse(parser.GetArg2()));
                    }
                    if (commandType == CommandType.C_ARITHMETIC)
                    {
                        codeWriter.WriteArithmetic(parser.GetArg1());
                    }

                }
            }
        }
    }
}