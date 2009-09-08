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
        private Dictionary<string,int> segmentLookUpTable = null;

        public CodeWriter(string outputFilePath)
        {
            this.fileStream = new FileStream(outputFilePath, FileMode.Create);
            this.streamWriter = new StreamWriter(fileStream);

            this.segmentLookUpTable = this.SetUpSegmentLookUpTable();
        }

        public string VmFileName
        {
            get { return this.vmFileName; }
            set { this.vmFileName = value; }
        }

        /// <summary>
        /// Writes the arithmetic.
        /// </summary>
        /// <param name="command">The command.</param>
        public void WriteArithmetic(string command)
        {
            switch (command)
            {
                case("add"):
                    {
                        this.WriteAdd();
                        break;
                    }
                case ("sub"):
                    {
                        this.WriteSubtract();
                        break;
                    }
                case ("eq"):
                    {
                        this.WriteEquality();
                        break;
                    }
                case ("neg"):
                    {
                        this.WriteNegate();
                        break;
                    }
                case ("and"):
                    {
                        this.WriteAnd();
                        break;
                    }
                case ("or"):
                    {
                        this.WriteOr();
                        break;
                    }
                case ("not"):
                    {
                        this.WriteNot();
                        break;
                    }
                case ("gt"):
                    {
                        this.WriteGreaterThan();
                        break;
                    }
                case ("lt"):
                    {
                        this.WriteLessThan();
                        break;
                    }
            }
            
        }

        public void WritePushPop(CommandType commandType, string segment, int index)
        {
            if (commandType == CommandType.C_PUSH)
            {
                if (segment == "constant")
                {
                    this.WritePushConstant(index);
                }
                else if (segment == "temp" || segment == "pointer")
                {
                    this.WritePushTempOrPointer(segment, index);
                }
                else 
                {
                    this.WritePushSegment(segment, index);
                }
            }
            if(commandType == CommandType.C_POP)
            {
                if (segment == "temp" || segment == "pointer")
                {
                    this.WritePopTempOrPointer(segment, index);
                }
                else
                {
                    this.WritePop(segment, index);
                }
            }
        }

        /// <summary>
        /// Just acts as a translator between local=LCL,this=THIS
        /// </summary>
        private Dictionary<string, int> SetUpSegmentLookUpTable()
        {
            Dictionary<string, int> lookUpTable = new Dictionary<string, int>();

            lookUpTable.Add("local", 1);
            lookUpTable.Add("argument", 2);
            lookUpTable.Add("this", 3);
            lookUpTable.Add("that", 4);
            lookUpTable.Add("R13", 13);
            lookUpTable.Add("R14", 14);
            lookUpTable.Add("R15", 15);
            return lookUpTable;
        }

        #region WriteAssemblyMethods

        private void WriteAdd()
        {
            streamWriter.WriteLine(@"//Write Add");
            // pop the operands into R14 & R15, I would expect them to be at the top of the stack
            this.WritePop("R14", 0);
            this.WritePop("R15", 0);

            streamWriter.WriteLine(@"//still in write add");
            streamWriter.WriteLine(@"@R14"); //Copy R14 to D and add it to R15, result is therefore in R15
            streamWriter.WriteLine(@"D=M");
            streamWriter.WriteLine(@"@R15");
            streamWriter.WriteLine(@"M=M+D");

            // R15 has the sum result, although I orginally thought that push segment
            // would only be used to push from memory segments, I can push from addresses aswell
            this.WritePushSegment("R15", 0);
        }

        private void WriteSubtract()
        {
            // pop the operands into R14 & R15, I would expect them to be at the top of the stack
            this.WritePop("R14", 0);
            this.WritePop("R15", 0);

            streamWriter.WriteLine(@"@R14"); //Copy R14 to D and add it to R15, result is therefore in R15
            streamWriter.WriteLine(@"D=M");
            streamWriter.WriteLine(@"@R15");
            streamWriter.WriteLine(@"M=M-D");

            // R15 has the sub result so push it onto the stack
            this.WritePushSegment("R15", 0);
        }

        private void WriteEquality()
        {
            streamWriter.WriteLine(@"//Write equality"); 
            this.WritePop("R14", 0);
            this.WritePop("R15", 0);

            // subtract and check for zero - this checks if they are equal
            streamWriter.WriteLine(@"@R14"); 
            streamWriter.WriteLine(@"D=M");
            streamWriter.WriteLine(@"@R15");
            streamWriter.WriteLine(@"M=M-D");
            streamWriter.WriteLine(@"D=M"); // copy subtraction result to D
            streamWriter.WriteLine(@"@EQUAL");
            streamWriter.WriteLine(@"D;JEQ");//jump to EQUAL if zero

            streamWriter.WriteLine(@"(NOTEQUAL)");
            streamWriter.WriteLine(@"@R15");
            streamWriter.WriteLine(@"M=0");// push zero onto stack for false (we push at end of method
            this.streamWriter.WriteLine("@END");
            this.streamWriter.WriteLine("0;JMP");

            streamWriter.WriteLine(@"(EQUAL)");
            streamWriter.WriteLine(@"@R15");
            streamWriter.WriteLine(@"M=-1");// push -1 onto stack for true (we push at end of method)
            

            this.streamWriter.WriteLine(@"(END)");
            this.WritePushSegment("R15", 0);

            streamWriter.WriteLine(@"//End Write equality"); 

        }

        private void WriteNegate()
        {
            this.WritePop("R14", 0);

            streamWriter.WriteLine(@"@R14"); 
            streamWriter.WriteLine(@"M=-M");

            this.WritePushSegment("R14", 0);
        }

        private void WriteAnd()
        {
            this.WritePop("R14", 0);
            this.WritePop("R15", 0);

            streamWriter.WriteLine(@"@R14");
            streamWriter.WriteLine(@"D=M");
            streamWriter.WriteLine(@"@R15");
            streamWriter.WriteLine(@"M=D&M");

            this.WritePushSegment("R15", 0);
        }

        private void WriteOr()
        {
            this.WritePop("R14", 0);
            this.WritePop("R15", 0);

            streamWriter.WriteLine(@"@R14");
            streamWriter.WriteLine(@"D=M");
            streamWriter.WriteLine(@"@R15");
            streamWriter.WriteLine(@"M=D|M");

            this.WritePushSegment("R15", 0);
        }

        private void WriteNot()
        {
            this.WritePop("R14", 0);

            streamWriter.WriteLine(@"@R14");
            streamWriter.WriteLine(@"M=!M");

            this.WritePushSegment("R14", 0);
        }

        // x > y
        private void WriteGreaterThan()
        {
            this.WritePop("R14", 0); //y
            this.WritePop("R15", 0); //x

            streamWriter.WriteLine(@"@R15");
            streamWriter.WriteLine(@"D=M");
            streamWriter.WriteLine(@"@R14");
            streamWriter.WriteLine(@"M=D-M"); // R15 -R14
            streamWriter.WriteLine(@"D=M"); //store result in D
            streamWriter.WriteLine(@"@GTTRUE");
            streamWriter.WriteLine(@"D;JGT"); // check if result is greater than zero if so R15 is greater than R14

            streamWriter.WriteLine(@"(GTFALSE)");
            streamWriter.WriteLine(@"@R15");
            streamWriter.WriteLine(@"M=-1");
            streamWriter.WriteLine(@"@ENDGT");
            streamWriter.WriteLine(@"0;JMP");

            streamWriter.WriteLine(@"(GTTRUE)");
            streamWriter.WriteLine(@"@R15");
            streamWriter.WriteLine(@"M=-1");

            streamWriter.WriteLine(@"(ENDGT)");

            this.WritePushSegment("R15", 0);
        }

        // x < y
        private void WriteLessThan()
        {
            this.WritePop("R14", 0); //y
            this.WritePop("R15", 0); //x

            streamWriter.WriteLine(@"@R15");
            streamWriter.WriteLine(@"D=M");
            streamWriter.WriteLine(@"@R14");
            streamWriter.WriteLine(@"M=D-M"); // R15 -R14
            streamWriter.WriteLine(@"D=M"); //store result in D
            streamWriter.WriteLine(@"@LTTRUE");
            streamWriter.WriteLine(@"D;JLT"); // check if result is greater than zero if so R15 is greater than R14

            streamWriter.WriteLine(@"(LTFALSE)");
            streamWriter.WriteLine(@"@R15");
            streamWriter.WriteLine(@"M=0");
            streamWriter.WriteLine(@"@ENDLT");
            streamWriter.WriteLine(@"0;JMP");

            streamWriter.WriteLine(@"(LTTRUE)");
            streamWriter.WriteLine(@"@R15");
            streamWriter.WriteLine(@"M=-1");

            streamWriter.WriteLine(@"(ENDLT)");

            this.WritePushSegment("R15", 0);
        }

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

        private void WritePushTempOrPointer(string seg, int index)
        {
            int address;

            if (seg == "pointer")
            {
                address = 3;
            }
            else //temp
            {
                address = 5;
            }

            address = address + index;

            streamWriter.WriteLine(@"@" + address); // copy contents into D
            streamWriter.WriteLine(@"D=M");

            streamWriter.WriteLine(@"@SP"); // goto top of stack
            streamWriter.WriteLine(@"A=M");

            streamWriter.WriteLine(@"M=D"); // copy D into top of stack

            this.IncrementStackPointer();
        }

        private void WritePushSegment(string seg, int index)
        {
            int segmentBaseAddress = this.segmentLookUpTable[seg];
            int segmentIndexAddress = segmentBaseAddress + index;

            // pointer and temp are handled differently than the other segments

            streamWriter.WriteLine(@"@" + segmentIndexAddress);
            streamWriter.WriteLine(@"D=M"); 
            streamWriter.WriteLine(@"@SP");
            streamWriter.WriteLine(@"A=M");
            streamWriter.WriteLine(@"M=D");

            this.IncrementStackPointer();
        }

        private void WritePop(string seg, int index)
        {
            //BRIAN!!!!!!!!!!!!!!
            //Here it looks like you have to store the seg[index]
            //e.g temp[10] actually means get address that local points to + 10!!!!

            int segmentBaseAddress = this.segmentLookUpTable[seg];
            int segmentIndexAddress = segmentBaseAddress + index;

            streamWriter.WriteLine(@"//Write Pop "+seg+" "+index);
            streamWriter.WriteLine(@"@SP");
            streamWriter.WriteLine(@"A=M-1");
            streamWriter.WriteLine(@"D=M");

            streamWriter.WriteLine(@"@" + segmentIndexAddress);
            streamWriter.WriteLine(@"M=D");

            this.DecrementStackPointer();
            streamWriter.WriteLine(@"//End Write Pop " + seg + " " + index);
        }

        private void WritePopTempOrPointer(string seg, int index)
        {
            int address;

            if (seg == "pointer")
            {
                address = 3;
            }
            else //temp
            {
                address = 5;
            }

            address = address + index;

            streamWriter.WriteLine("@SP");
            streamWriter.WriteLine("A=M-1"); //not minus one for top value
            streamWriter.WriteLine("D=M");//copy contents to D

            streamWriter.WriteLine("@"+address); 
            streamWriter.WriteLine("M=D");

            this.DecrementStackPointer();
        }

        private void IncrementStackPointer()
        {
            streamWriter.WriteLine(@"@SP");
            streamWriter.WriteLine(@"M=M+1");
        }

        private void DecrementStackPointer()
        {
            streamWriter.WriteLine(@"@SP");
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
