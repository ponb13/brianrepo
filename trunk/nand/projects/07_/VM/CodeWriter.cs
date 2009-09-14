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
        private Dictionary<string, int> segmentLookUpTable = null;

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
        /// Writes the assembly code that is the 
        /// translation of the given arithmetic command.
        /// </summary>
        /// <param name="command">The command.</param>
        public void WriteArithmetic(string command)
        {
            switch (command)
            {
                case ("add"):
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

        /// <summary>
        /// Writes the assembly code that is hte translation of the given command.
        /// </summary>
        /// <param name="commandType">Type of the command.</param>
        /// <param name="segment">The segment.</param>
        /// <param name="index">The index.</param>
        public void WritePushPop(CommandType commandType, string segment, int index)
        {
            if (commandType == CommandType.C_PUSH)
            {
                if (segment == "constant")
                {
                    this.WritePushConstant(index);
                }
                else if (segment == "static")
                {
                    this.WritePushStatic(index);
                }
                else if (segment == "temp" || segment == "pointer")
                {
                    // handles push temp / pointer
                    this.WritePushTempOrPointer(segment, index);
                }
                else
                {
                    // handles push local / arg / this / that
                    this.WritePushSegmentIndex(segment, index);
                }
            }
            if (commandType == CommandType.C_POP)
            {
                if (segment == "temp" || segment == "pointer")
                {
                    // handles push temp / pointer
                    this.WritePopTempOrPointer(segment, index);
                }
                else if (segment == "static")
                {
                    this.WritePopStatic(index);
                }
                else
                {
                    // handles pop to local / arg / this / that [index]
                    this.WritePopSegmentIndex(segment, index);
                }
            }
        }

        /// <summary>
        /// Just acts as a translator between local=LCL,this=THIS etc
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
            lookUpTable.Add("pointer", 3);
            lookUpTable.Add("temp", 5);
            return lookUpTable;
        }

        #region WriteArthemeticCommands

        private void WriteAdd()
        {
            streamWriter.WriteLine(@"//Write Add");
            // pop the operands into R14 & R15, I would expect them to be at the top of the stack
            this.WritePopToR("R14");
            this.WritePopToR("R15");

            streamWriter.WriteLine(@"//still in write add");
            streamWriter.WriteLine(@"@R14"); //Copy R14 to D and add it to R15, result is therefore in R15
            streamWriter.WriteLine(@"D=M");
            streamWriter.WriteLine(@"@R15");
            streamWriter.WriteLine(@"M=M+D");

            // R15 has the sum result, although I orginally thought that push segment
            // would only be used to push from memory segments, I can push from addresses aswell
            this.PushR("R15");
        }

        private void WriteSubtract()
        {
            // pop the operands into R14 & R15, I would expect them to be at the top of the stack
            this.WritePopToR("R14");
            this.WritePopToR("R15");

            streamWriter.WriteLine(@"@R14"); //Copy R14 to D and add it to R15, result is therefore in R15
            streamWriter.WriteLine(@"D=M");
            streamWriter.WriteLine(@"@R15");
            streamWriter.WriteLine(@"M=M-D");

            // R15 has the sub result so push it onto the stack
            this.PushR("R15");
        }

        private void WriteEquality()
        {
            streamWriter.WriteLine(@"//Write equality");
            this.WritePopToR("R14");
            this.WritePopToR("R15");

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
            this.PushR("R15");

            streamWriter.WriteLine(@"//End Write equality");

        }

        private void WriteNegate()
        {
            this.WritePopToR("R14");

            streamWriter.WriteLine(@"@R14");
            streamWriter.WriteLine(@"M=-M");

            this.PushR("R14");
        }

        private void WriteAnd()
        {
            this.WritePopToR("R14");
            this.WritePopToR("R15");

            streamWriter.WriteLine(@"@R14");
            streamWriter.WriteLine(@"D=M");
            streamWriter.WriteLine(@"@R15");
            streamWriter.WriteLine(@"M=D&M");

            this.PushR("R15");
        }

        private void WriteOr()
        {
            this.WritePopToR("R14");
            this.WritePopToR("R15");

            streamWriter.WriteLine(@"@R14");
            streamWriter.WriteLine(@"D=M");
            streamWriter.WriteLine(@"@R15");
            streamWriter.WriteLine(@"M=D|M");

            this.PushR("R15");
        }

        private void WriteNot()
        {
            this.WritePopToR("R14");

            streamWriter.WriteLine(@"@R14");
            streamWriter.WriteLine(@"M=!M");

            this.PushR("R14");
        }

        // x > y
        private void WriteGreaterThan()
        {
            this.WritePopToR("R14"); //y
            this.WritePopToR("R15"); //x

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

            this.PushR("R15");
        }

        // x < y
        private void WriteLessThan()
        {
            this.WritePopToR("R14"); //y
            this.WritePopToR("R15"); //x

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

            this.PushR("R15");
        }

        #endregion 

        #region StackCommands

        private void WritePushConstant(int index)
        {
            streamWriter.WriteLine(@"@" + index);
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

        private void WritePushSegmentIndex(string seg, int index)
        {
            //May need to write push R15 etc
            int segmentBaseAddress = this.segmentLookUpTable[seg];

            this.StoreSegmentPlusIndexPointerInR13(segmentBaseAddress, index);

            this.streamWriter.WriteLine(@"@R13");
            this.streamWriter.WriteLine(@"A=M");
            this.streamWriter.WriteLine(@"D=M");
            this.streamWriter.WriteLine(@"@SP");
            this.streamWriter.WriteLine(@"A=M");
            this.streamWriter.WriteLine(@"M=D");

            this.IncrementStackPointer();
        }

        /// <summary>
        /// Writes the assembly code for pushing a value into one of the mapped R
        /// memory locations e.g. R15, R14, R13
        /// Only used internally, i.e. you won't ever see VM code to push something onto R15
        /// </summary>
        /// <param name="R"></param>
        private void PushR(string R)
        {
            this.streamWriter.WriteLine(@"@" + R);
            this.streamWriter.WriteLine(@"D=M");
            this.streamWriter.WriteLine(@"@SP");
            this.streamWriter.WriteLine(@"A=M");
            this.streamWriter.WriteLine(@"M=D");

            this.IncrementStackPointer();

        }

        /// <summary>
        /// Writes the pop to segment.
        /// Should only be used with LCL ARG THIS THAT segment
        /// </summary>
        /// <param name="seg">The seg.</param>
        /// <param name="index">The index.</param>
        private void WritePopSegmentIndex(string seg, int index)
        {
            int segement = this.segmentLookUpTable[seg];

            this.StoreSegmentPlusIndexPointerInR13(segement, index);

            this.streamWriter.WriteLine(@"//WritePopSegmentIndex ");
            this.streamWriter.WriteLine(@"@SP");//copy stack top to D
            this.streamWriter.WriteLine(@"A=M-1");
            this.streamWriter.WriteLine(@"D=M");

            this.streamWriter.WriteLine(@"@R13");//get pointer to seg[index] from R13
            this.streamWriter.WriteLine(@"A=M");
            this.streamWriter.WriteLine(@"M=D");//make seg[index] = contents of D (which should have value from top of stack.

            this.DecrementStackPointer();

            this.streamWriter.WriteLine(@"//End WritePopSegmentIndex ");
        }

        /// <summary>
        ///  does what it says on the tin!
        ///  segmentBaseAddress + index and store in R13
        /// </summary>
        /// <param name="segmentBase"></param>
        /// <param name="index"></param>
        private void StoreSegmentPlusIndexPointerInR13(int segmentBase, int index)
        {
            this.streamWriter.WriteLine(@"//Begin StoreSegmentPlusIndexPointerInR13 ");
            this.streamWriter.WriteLine(@"@" + index);
            this.streamWriter.WriteLine(@"D=A"); // D = index 

            this.streamWriter.WriteLine(@"@" + segmentBase);
            this.streamWriter.WriteLine(@"A=M+D");// now at seg[index]
            this.streamWriter.WriteLine(@"D=A"); //save this pointer in @R13
            this.streamWriter.WriteLine(@"@R13");
            this.streamWriter.WriteLine(@"M=D");
            this.streamWriter.WriteLine(@"//End StoreSegmentPlusIndexPointerInR13 ");
        }

        /// <summary>
        /// Used to Pop to an R Register
        /// </summary>
        /// <param name="R"></param>
        private void WritePopToR(string R)
        {
            int address = this.segmentLookUpTable[R];
            streamWriter.WriteLine(@"//Begin Write Pop " + R);
            streamWriter.WriteLine(@"@SP");
            streamWriter.WriteLine(@"A=M-1");
            streamWriter.WriteLine(@"D=M");

            streamWriter.WriteLine(@"@" + address);
            streamWriter.WriteLine(@"M=D");

            this.DecrementStackPointer();
            streamWriter.WriteLine(@"// End Write Pop " + R);
        }

        /// <summary>
        /// writes to temp or pointer
        /// temp and pointer differ from THIS,THAT, LCL, ARG
        /// in that temp does not contain a pointer to elsewhere
        /// so that temp[3] go to location means 5+3
        /// </summary>
        /// <param name="seg">The seg.</param>
        /// <param name="index">The index.</param>
        private void WritePopTempOrPointer(string seg, int index)
        {
            int baseAddress = this.segmentLookUpTable[seg];

            int address = baseAddress + index;

            this.streamWriter.WriteLine("@SP");
            this.streamWriter.WriteLine("A=M-1"); //minus one for top value
            this.streamWriter.WriteLine("D=M");//copy contents to D

            this.streamWriter.WriteLine("@" + address);
            this.streamWriter.WriteLine("M=D");

            this.DecrementStackPointer();
        }

        /// <summary>
        /// Writes the push static.
        /// </summary>
        /// <param name="index">The index.</param>
        private void WritePushStatic(int index)
        {
            this.streamWriter.WriteLine(@"@"+this.VmFileName+"."+index);
            this.streamWriter.WriteLine("D=M");
            this.streamWriter.WriteLine(@"@SP");
            this.streamWriter.WriteLine(@"A=M");
            this.streamWriter.WriteLine(@"M=D");
            this.IncrementStackPointer();
        }

        private void WritePopStatic(int index)
        {
            this.streamWriter.WriteLine("@SP");
            this.streamWriter.WriteLine("A=M-1");
            this.streamWriter.WriteLine("D=M");
            this.streamWriter.WriteLine(@"@" + this.VmFileName + "." + index);
            this.streamWriter.WriteLine("M=D");

            this.DecrementStackPointer();
        }



        /// <summary>
        /// Increments the stack pointer.
        /// </summary>
        private void IncrementStackPointer()
        {
            streamWriter.WriteLine(@"@SP");
            streamWriter.WriteLine(@"M=M+1");
        }

        /// <summary>
        /// Decrements the stack pointer.
        /// </summary>
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
