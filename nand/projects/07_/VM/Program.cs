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
            string intputFilePath = @"..\..\..\MemoryAccess\StaticTest\StaticTest.vm";
            string outPutFilePath = @"..\..\..\MemoryAccess\StaticTest\StaticTest.asm";



            Program.Process(intputFilePath, outPutFilePath);
        }

        /// <summary>
        /// Processes the specified input file and writes to output path.
        /// </summary>
        /// <param name="inputPath">The input path.</param>
        /// <param name="outputPath">The output path.</param>
        public static void Process(string inputPath, string outputPath)
        {
            using (Parser parser = new Parser(inputPath))
            using (CodeWriter codeWriter = new CodeWriter(outputPath))
            {
                codeWriter.VmFileName = Path.GetFileNameWithoutExtension(inputPath);
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
