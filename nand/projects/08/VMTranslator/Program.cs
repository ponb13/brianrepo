using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace VMTranslator
{
    class Program
    {
        static void Main(string[] args)
        {
            string intputFilePath = @"..\..\..\FunctionCalls\SimpleFunction\SimpleFunction.vm";
            string outPutFilePath = @"..\..\..\FunctionCalls\SimpleFunction\SimpleFunction.asm";

            IList<string> linesOfAssemblyCode = new List<string>();

            if (Program.PathIsDirectory(intputFilePath))
            {
                foreach (string file in Directory.GetFiles(Path.GetDirectoryName(intputFilePath)))
                {
                    if (Path.GetExtension(file) == ".vm")
                    {
                        Program.ProcessFile(Path.GetFullPath(file), linesOfAssemblyCode);
                    }
                }
            }
            else
            {
                Program.ProcessFile(intputFilePath, linesOfAssemblyCode);
            }


            Program.WriteAssemblyCodeToFile(linesOfAssemblyCode, outPutFilePath);
        }

        /// <summary>
        /// Processes the specified input path.
        /// </summary>
        /// <param name="inputPath">The input path.</param>
        private static void ProcessFile(string inputPath, IList<string> linesOfAssemblyCode)
        {
            using (Parser parser = new Parser(inputPath))
            {
                CodeWriter codeWriter = new CodeWriter(linesOfAssemblyCode);
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
                    if (commandType == CommandType.C_LABEL)
                    {
                        codeWriter.WriteLabel(parser.GetArg1());
                    }
                    if (commandType == CommandType.C_GOTO)
                    {
                        codeWriter.WriteGoto(parser.GetArg1());
                    }
                    if (commandType == CommandType.C_IF)
                    {
                        codeWriter.WriteIf(parser.GetArg1());
                    }
                    if (commandType == CommandType.C_CALL)
                    {
                        codeWriter.WriteCall(parser.GetArg1(), int.Parse(parser.GetArg2()));
                    }
                }
            }
        }

        /// <summary>
        /// Writes the assembly code to file.
        /// </summary>
        /// <param name="linesOfAssemblyCode">The lines of assembly code.</param>
        /// <param name="outputPath">The output path.</param>
        private static void WriteAssemblyCodeToFile(IList<string> linesOfAssemblyCode, string outputPath)
        {
            using (FileStream fileStream = new FileStream(outputPath, FileMode.Create))
            using (StreamWriter writer = new StreamWriter(fileStream))
            {
                foreach (string line in linesOfAssemblyCode)
                {
                    writer.WriteLine(line);
                }
            }
        }

        private static bool PathIsDirectory(string filePath)
        {
            FileAttributes attr = File.GetAttributes(filePath);
            //detect whether its a directory or file
            return ((attr & FileAttributes.Directory) == FileAttributes.Directory);
        }

    }
}
