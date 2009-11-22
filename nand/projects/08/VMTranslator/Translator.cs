using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace VMTranslator
{
    /// <summary>
    /// Acts as the glue between the Parser and the Codewriter
    /// </summary>
    public class Translator
    {
        /// <summary>
        /// the path to a single vm file
        /// or a directory
        /// </summary>
        private string intputPath;

        /// <summary>
        /// the path that the translated vm code will be written to
        /// i.e. where the single assembly code file will be written to.
        /// </summary>
        private string outputPath;

        /// <summary>
        /// List of assembly code lines.
        /// Instead of writing directly to a stream
        /// just build a list of lines and write them out
        /// all at once when required.
        /// </summary>
        private IList<string> assemblyCode = new List<string>();

        /// <summary>
        /// Initializes a new instance of the <see cref="Translator"/> class.
        /// </summary>
        /// <param name="path">The path to a single file or directory</param>
        public Translator(string inputPath, string outputPath)
        {
            this.intputPath = inputPath;
            this.outputPath = outputPath;
        }

        /// <summary>
        /// Begins the translation of the loaded file / directory
        /// </summary>
        public void Translate()
        {
            this.WriteInitCode();

            if (this.PathIsDirectory())
            {
                this.ProcessDirectory();
            }
            else
            {
                this.ProcessFile(this.intputPath);
            }

            this.WriteAssemblyCodeToFile();
        }

        /// <summary>
        /// Writes the the boot strap code asssembly code.
        /// i.e. sets the stack pointer 
        /// and calls Sys.init
        /// </summary>
        private void WriteInitCode()
        {
            this.assemblyCode.Add("@256");
            this.assemblyCode.Add("D=A");
            this.assemblyCode.Add("@SP");
            this.assemblyCode.Add("M=D");

            CodeWriter cd = new CodeWriter(this.assemblyCode);
            cd.WriteCall("Sys.init", 0);
        }

        /// <summary>
        /// Processes a directory containing one or more vm files.
        /// Only files with .vm will be processed.
        /// </summary>
        private void ProcessDirectory()
        {
            foreach (string file in Directory.GetFiles(Path.GetDirectoryName(this.intputPath)))
            {
                if (Path.GetExtension(file) == ".vm")
                {
                    this.ProcessFile(Path.GetFullPath(file));
                }
            }
        }

        /// <summary>
        /// Processes the specified input path.
        /// </summary>
        /// <param name="inputPath">The input path.</param>
        private void ProcessFile(string inputPath)
        {
            using (Parser parser = new Parser(inputPath))
            {
                CodeWriter codeWriter = new CodeWriter(this.assemblyCode);
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
                    if (commandType == CommandType.C_FUNCTION)
                    {
                        codeWriter.WriteFunction(parser.GetArg1(), int.Parse(parser.GetArg2()));
                    }
                    if (commandType == CommandType.C_RETURN)
                    {
                        codeWriter.WriteReturn();
                    }
                }
            }
        }

        /// <summary>
        /// Checks if currently set input path is a directory or a file.
        /// </summary>
        /// <returns></returns>
        private bool PathIsDirectory()
        {
            FileAttributes attr = File.GetAttributes(this.intputPath);
            //detect whether its a directory or file
            return ((attr & FileAttributes.Directory) == FileAttributes.Directory);
        }

        /// <summary>
        /// Writes the assembly code to file.
        /// </summary>
        private void WriteAssemblyCodeToFile()
        {
            File.Delete(outputPath);

            using (FileStream fileStream = new FileStream(this.outputPath, FileMode.Create, FileAccess.Write))
            using (StreamWriter writer = new StreamWriter(fileStream))
            {
                foreach (string line in this.assemblyCode)
                {
                    writer.WriteLine(line);
                }
            }
        }
    }
}
