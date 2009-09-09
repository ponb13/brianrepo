using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;


namespace VM
{
    class Program
    {
        static void Main(string[] args)
        {
            string filePath = @"..\..\..\MemoryAccess\PointerTest\PointerTest.vm";
            string outPutFilePath = @"..\..\..\MemoryAccess\PointerTest\PointerTest.asm";

            //string filePath = @"..\..\..\StackArithmetic\SimpleAdd\SimpleAdd.vm";
            //string outPutFilePath = @"..\..\..\StackArithmetic\SimpleAdd\SimpleAdd.asm";

            using (Parser parser = new Parser(filePath))
            using (CodeWriter codeWriter = new CodeWriter(outPutFilePath))
            {
                codeWriter.VmFileName = Path.GetFileNameWithoutExtension(filePath);
                while (parser.HasMoreCommands())
                {
                    parser.Advance();
                    CommandType commandType = parser.GetCommandType();

                    if (commandType == CommandType.C_PUSH || commandType == CommandType.C_POP)
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
