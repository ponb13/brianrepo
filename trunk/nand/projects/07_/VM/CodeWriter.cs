using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace VM
{
    public class CodeWriter : IDisposable
    {
        private FileStream fileStream = null;
        private StreamWriter streamWriter;
        private string vmFileName;
        private Dictionary<string, string> segmentLookUpTable = null;

        public CodeWriter(string outputFilePath)
        {
            this.fileStream = new FileStream(outputFilePath, FileMode.Create);
            this.streamWriter = new StreamWriter(fileStream);

            this.SetUpSegmentLookUpTable();
        }

        public void SetVmFileName(string fileName)
        {
            this.vmFileName = fileName;
        }

        public void WriteArithmetic(string command)
        {
            this.WritePop("temp", 0);
            this.WritePop("temp", 1);



        }

        public void WritePushPop(CommandType commandType, string segment, int index)
        {
            if (commandType == CommandType.C_PUSH)
            {
                if (segment == "constant")
                {
                    this.WritePushConstant(index);
                }
                else
                {
                    this.WritePushSegment(segment, index);
                }
            }
            if(commandType == CommandType.C_POP)
            {
                this.WritePop(segment, index);
            }
        }

        /// <summary>
        /// Just acts as a translator between local=LCL,this=THIS
        /// </summary>
        private Dictionary<string, string> SetUpSegmentLookUpTable()
        {
            Dictionary<string, string> lookUpTable = new Dictionary<string, string>();

            lookUpTable.Add("temp", "TEMP");
            lookUpTable.Add("this", "THIS");
            lookUpTable.Add("that", "THAT");
            lookUpTable.Add("local", "LCL");
            lookUpTable.Add("arguement", "ARG");

            return lookUpTable;
        }

        #region WriteAssemblyMethods
        private void WritePushConstant(int index)
        {
            streamWriter.WriteLine(@"@"+index);
            streamWriter.WriteLine(@"D=A");
            streamWriter.WriteLine(@"@SP");
            streamWriter.WriteLine(@"A=M");
            streamWriter.WriteLine(@"M=D");
            streamWriter.WriteLine(@"@SP");
            streamWriter.WriteLine(@"M=M+1");
        }

        private void WritePushSegment(string seg, int index)
        {
            string segment = this.segmentLookUpTable[seg];

            streamWriter.WriteLine(@"@" + index);//store segment index in D
            streamWriter.WriteLine(@"D=A");
            streamWriter.WriteLine(@"@" + segment);
            streamWriter.WriteLine(@"A=A+D");//point to segment[index]
            streamWriter.WriteLine(@"D=A");//store address in D
            streamWriter.WriteLine(@"@R13");//R13 is a general purpose reg, use it to store pointer to segment[index]
            streamWriter.WriteLine(@"M=D");//save pointer to variable
            streamWriter.WriteLine(@"@SP");//get value out of stack and store in D
            streamWriter.WriteLine(@"A=M");
            streamWriter.WriteLine(@"D=M");
            streamWriter.WriteLine(@"@R13");// get get pointer segment[index] from general purchase reg
            streamWriter.WriteLine(@"A=M"); // goto pointer address (segment[index])
            streamWriter.WriteLine(@"D=M"); // store contents of segment[index] in d
            streamWriter.WriteLine(@"@SP");
            streamWriter.WriteLine(@"A=M");
            streamWriter.WriteLine(@"M=D");
            streamWriter.WriteLine(@"@SP");//increment sp
            streamWriter.WriteLine(@"M=M+1");
        }

        private void WritePop(string seg, int index)
        {
            string segment = this.segmentLookUpTable[seg];

            streamWriter.WriteLine(@"@"+index);//store segment index in D
            streamWriter.WriteLine(@"D=A");
            streamWriter.WriteLine(@"@"+segment);
            streamWriter.WriteLine(@"A=A+D");//point to segment[index]
            streamWriter.WriteLine(@"D=A");//store address in D
            streamWriter.WriteLine(@"@R13");//R13 is a general purpose reg, use it to store pointer to segment[index]
            streamWriter.WriteLine(@"M=D");//save pointer to variable
            streamWriter.WriteLine(@"@SP");//get value out of stack and store in D
            streamWriter.WriteLine(@"A=M");
            streamWriter.WriteLine(@"D=M");
            streamWriter.WriteLine(@"@R13");//get get pointer segment[index] from general purchase reg
            streamWriter.WriteLine(@"A=M"); // goto pointer address (segment[index])
            streamWriter.WriteLine(@"M=D");// store value from top of stack into segment[index]
            streamWriter.WriteLine(@"@SP");//decrement sp
            streamWriter.WriteLine(@"M=M-1");
        }

        #endregion

        #region IDisposable Members

        void IDisposable.Dispose()
        {
            this.streamWriter.Flush();
            this.fileStream.Close();
        }

        #endregion
    }
}
