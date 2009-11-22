using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace VMTranslator
{
    /// <summary>
    /// Translates vm commands into hack assembly language
    /// </summary>
    public class CodeWriter
    {
        #region private variables

        /// <summary>
        /// store the lines of generated vm code as a list of strings
        /// little bit simpler than constantly writing to an output stream
        /// </summary>
        private IList<string> linesOfCode = null;

        /// <summary>
        /// See SetUpSegmentLookUpTable method in this class.
        /// </summary>
        private Dictionary<string, int> segmentLookUpTable = null;
        
        /// <summary>
        /// The name of the vm function that is currently executing.
        /// </summary>
        private string currentExecutingFunction;
        

        // when writing equality,greater than and less than statements
        // we use labels, we can't keep using the same labels
        // so keep a count of each and append to label to make each label unique
        private int eq_count;
        private int lt_count;
        private int gt_count;

        #endregion

        #region public properties
        /// <summary>
        /// Gets or sets the current executing function name.
        /// usful for building label names in hack assembly code.
        /// </summary>
        /// <value>The current executing function.</value>
        public string CurrentExecutingFunction
        {
            get 
            {
                if (String.IsNullOrEmpty(this.currentExecutingFunction))
                {
                    this.currentExecutingFunction = "undefined";
                    return this.currentExecutingFunction;
                }
                else
                {
                    return this.currentExecutingFunction;
                }
            }
            set { this.currentExecutingFunction = value; }
        }

        /// <summary>
        /// Gets or sets the name of the vm file.
        /// </summary>
        /// <value>The name of the vm file.</value>
        public string VmFileName
        {
            get;
            set;
        }
        #endregion

        #region ctors
        /// <summary>
        /// Initializes a new instance of the <see cref="CodeWriter"/> class.
        /// </summary>
        /// <param name="linesOfAssemblyCode">The lines of assembly code.</param>
        public CodeWriter(IList<string> linesOfAssemblyCode)
        {
            this.linesOfCode = linesOfAssemblyCode;
            this.segmentLookUpTable = this.SetUpSegmentLookUpTable();
        }
        #endregion

        #region public methods
        /// <summary>
        /// Writes the hack assembly code that is the 
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
        /// Writes the assembly code that effects the label command
        /// </summary>
        /// <param name="labelName">Name of the label.</param>
        public void WriteLabel(string labelName)
        {
            linesOfCode.Add("(" + this.VmFileName + "." + this.CurrentExecutingFunction + "$" + labelName + ")");
        }

        /// <summary>
        /// Writes the assembly code that effects the goto command
        /// </summary>
        /// <param name="labelName">Name of the label.</param>
        public void WriteGoto(string labelName)
        {
            linesOfCode.Add("@" + this.VmFileName + "." + this.CurrentExecutingFunction + "$" + labelName);
            linesOfCode.Add("0;JMP");
        }

        /// <summary>
        /// Writes assembly code that effects the if command.
        /// </summary>
        /// <param name="labelName">Name of the label.</param>
        public void WriteIf(string labelName)
        {
            this.linesOfCode.Add(@"@SP");
            this.linesOfCode.Add(@"A=M-1");
            linesOfCode.Add(@"D=M");
            this.DecrementStackPointer();
            this.linesOfCode.Add(@"@" + this.VmFileName + "." + this.CurrentExecutingFunction + "$" + labelName);
            this.linesOfCode.Add(@"D;JNE");
        }

        /// <summary>
        /// Writes assembly code that effects a declare function declaration command
        /// </summary>
        /// <param name="functionName">Name of the function.</param>
        /// <param name="?">The number of args.</param>
        public void WriteFunction(string functionName, int numberOfLocals)
        {
            linesOfCode.Add(@"//WriteFunction" + functionName);

            //set a label at the start of function
            linesOfCode.Add("("+functionName+")");

            // push 0 numberOfLocals times, remember when a function is called LCL is set to SP
            for (int i = numberOfLocals; i > 0; i--)
            {
                this.linesOfCode.Add("// pushing 0 for local var");
                this.WritePushConstant(0);
            }
        }

        /// <summary>
        /// Writes assembly code that effects a call function command
        /// </summary>
        /// <param name="functionName">Name of the function.</param>
        /// <param name="numberOfAgrs">The number of agrs.</param>
        public void WriteCall(string functionName, int numberOfAgrs)
        {
            linesOfCode.Add(@"//WriteCall " + functionName);
            // push return address
            string returnAddressLabel = Guid.NewGuid().ToString("N")+"return";

            this.linesOfCode.Add("@" + returnAddressLabel + "//pushing return address"); 
            this.linesOfCode.Add("D=A");
            this.linesOfCode.Add("@SP");
            this.linesOfCode.Add("A=M");
            this.linesOfCode.Add("M=D");
            this.IncrementStackPointer();

            this.SaveSegmentPointer_ForFunctionCall("LCL");
            this.SaveSegmentPointer_ForFunctionCall("ARG");
            this.SaveSegmentPointer_ForFunctionCall("THIS");
            this.SaveSegmentPointer_ForFunctionCall("THAT");

            // reset Arg = SP-numberOfArgs-5
            this.linesOfCode.Add("@SP");
            this.linesOfCode.Add("D=M");
            this.linesOfCode.Add("@ARG");
            this.linesOfCode.Add("M=D");
            this.linesOfCode.Add("@" + numberOfAgrs);
            this.linesOfCode.Add("D=A");
            this.linesOfCode.Add("@ARG");
            this.linesOfCode.Add("M=M-D");
            this.linesOfCode.Add("@5");
            this.linesOfCode.Add("D=A");
            this.linesOfCode.Add("@ARG");
            this.linesOfCode.Add("M=M-D");

            //LCL = SP
            this.linesOfCode.Add("@SP");
            this.linesOfCode.Add("D=M");
            this.linesOfCode.Add("@LCL");
            this.linesOfCode.Add("M=D");

            //goto f
            this.linesOfCode.Add("@" + functionName);
            this.linesOfCode.Add("0;JMP");

            //return address, this has already been pushed
            this.linesOfCode.Add(@"(" + returnAddressLabel + ")" + "//callers - return label - i.e. start executing from here once " + functionName + " completes");

            this.CurrentExecutingFunction = functionName;
        }

        /// <summary>
        /// Saves the segment pointer for function call
        /// i.e. push a pointer to a given segment 
        /// </summary>
        private void SaveSegmentPointer_ForFunctionCall(string segmentName)
        {
            linesOfCode.Add(@"//Save "+segmentName+" for function call");

            this.linesOfCode.Add("@" + segmentName);
            this.linesOfCode.Add("D=M");
            this.linesOfCode.Add("@SP");
            this.linesOfCode.Add("A=M");
            this.linesOfCode.Add("M=D");
            this.IncrementStackPointer();
        }

        /// <summary>
        /// Writes assembly code that returns from a function call.
        /// </summary>
        public void WriteReturn()
        {
            linesOfCode.Add(@"//WriteReturn");
            
            string frameLabel = "@FRAME";
            string returnAddressPointer = "@RET_Pointer" ;
            
            // FRAME = LCL
            this.linesOfCode.Add("@LCL");
            this.linesOfCode.Add("D=M");
            this.linesOfCode.Add(frameLabel);
            this.linesOfCode.Add("M=D");

            // RET pointer = *(FRAME-5)
            this.linesOfCode.Add("@5");
            this.linesOfCode.Add("D=A");
            this.linesOfCode.Add(frameLabel);
            this.linesOfCode.Add("D=M-D");
            this.linesOfCode.Add(returnAddressPointer);
            this.linesOfCode.Add("M=D");

            //pointer shizzles
            this.linesOfCode.Add(returnAddressPointer);
            this.linesOfCode.Add("A=M");
            this.linesOfCode.Add("D=M");
            this.linesOfCode.Add(returnAddressPointer);
            this.linesOfCode.Add("M=D");

            
            // POP to where ever ARG is pointing at - put return value  at top of stack
            this.linesOfCode.Add("@SP");
            this.linesOfCode.Add("A=M-1"); // got sp-1 to get value at top of stack
            this.linesOfCode.Add("D=M");
            this.linesOfCode.Add("@ARG");
            this.linesOfCode.Add("A=M");
            this.linesOfCode.Add("M=D");

            // Set SP = ARG+1
            this.linesOfCode.Add("@ARG");
            this.linesOfCode.Add("D=M");
            this.linesOfCode.Add("@SP");
            this.linesOfCode.Add("M=D+1");

            // Restore segments
            this.WriteRestoreSegmentForReturn("THAT", 1, frameLabel);
            this.WriteRestoreSegmentForReturn("THIS", 2, frameLabel);
            this.WriteRestoreSegmentForReturn("ARG", 3, frameLabel);
            this.WriteRestoreSegmentForReturn("LCL", 4, frameLabel);

            // goto RET
            this.linesOfCode.Add("@RET_Pointer");
            this.linesOfCode.Add("A=M");
            this.linesOfCode.Add("0;JMP");
        }
        #endregion

        #region Memory Segment mapping
        /// <summary>
        /// Maps memory location names to their actual addresses
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
        #endregion

        #region Arthemtic command
        private void WriteAdd()
        {
            linesOfCode.Add(@"//Write Add");
            // pop the operands into R14 & R15, I would expect them to be at the top of the stack
            this.WritePopToR("R14");
            this.WritePopToR("R15");

            linesOfCode.Add(@"//still in write add");
            linesOfCode.Add(@"@R14"); //Copy R14 to D and add it to R15, result is therefore in R15
            linesOfCode.Add(@"D=M");
            linesOfCode.Add(@"@R15");
            linesOfCode.Add(@"M=M+D");

            // R15 has the sum result, although I orginally thought that push segment
            // would only be used to push from memory segments, we can push from addresses aswell
            this.PushR("R15");
        }

        private void WriteSubtract()
        {
            // pop the operands into R14 & R15, I would expect them to be at the top of the stack
            this.WritePopToR("R14");
            this.WritePopToR("R15");

            linesOfCode.Add(@"@R14"); //Copy R14 to D and add it to R15, result is therefore in R15
            linesOfCode.Add(@"D=M");
            linesOfCode.Add(@"@R15");
            linesOfCode.Add(@"M=M-D");

            // R15 has the sub result so push it onto the stack
            this.PushR("R15");
        }

        private void WriteNegate()
        {
            this.WritePopToR("R14");

            linesOfCode.Add(@"@R14");
            linesOfCode.Add(@"M=-M");

            this.PushR("R14");
        }

        private void WriteAnd()
        {
            this.WritePopToR("R14");
            this.WritePopToR("R15");

            linesOfCode.Add(@"@R14");
            linesOfCode.Add(@"D=M");
            linesOfCode.Add(@"@R15");
            linesOfCode.Add(@"M=D&M");

            this.PushR("R15");
        }

        private void WriteOr()
        {
            this.WritePopToR("R14");
            this.WritePopToR("R15");

            linesOfCode.Add(@"@R14");
            linesOfCode.Add(@"D=M");
            linesOfCode.Add(@"@R15");
            linesOfCode.Add(@"M=D|M");

            this.PushR("R15");
        }

        private void WriteNot()
        {
            this.WritePopToR("R14");

            linesOfCode.Add(@"@R14");
            linesOfCode.Add(@"M=!M");

            this.PushR("R14");
        }

        // x > y
        private void WriteGreaterThan()
        {
            this.WritePopToR("R14"); //y
            this.WritePopToR("R15"); //x

            linesOfCode.Add(@"@R15");
            linesOfCode.Add(@"D=M");
            linesOfCode.Add(@"@R14");
            linesOfCode.Add(@"M=D-M"); // R15 -R14
            linesOfCode.Add(@"D=M"); //store result in D
            linesOfCode.Add(@"@GTTRUE_" + this.gt_count);
            linesOfCode.Add(@"D;JGT"); // check if result is greater than zero if so R15 is greater than R14

            linesOfCode.Add(@"@R15");
            linesOfCode.Add(@"M=-1");
            linesOfCode.Add(@"@ENDGT_" + this.gt_count);
            linesOfCode.Add(@"0;JMP");

            linesOfCode.Add(@"(GTTRUE_" + this.gt_count + ")");
            linesOfCode.Add(@"@R15");
            linesOfCode.Add(@"M=-1");

            linesOfCode.Add(@"(ENDGT_" + this.gt_count + ")");

            this.PushR("R15");

            this.gt_count++;
        }

        // x < y
        private void WriteLessThan()
        {
            this.linesOfCode.Add("// writing less than");
            this.WritePopToR("R14"); //y
            this.WritePopToR("R15"); //x

            linesOfCode.Add(@"@R15");
            linesOfCode.Add(@"D=M");
            linesOfCode.Add(@"@R14");
            linesOfCode.Add(@"M=D-M"); // R15 -R14
            linesOfCode.Add(@"D=M"); //store result in D
            linesOfCode.Add(@"@LTTRUE_" + this.lt_count);
            linesOfCode.Add(@"D;JLT"); // check if result is greater than zero if so R15 is greater than R14

            linesOfCode.Add(@"(LTFALSE_" + this.lt_count + ")");
            linesOfCode.Add(@"@R15");
            linesOfCode.Add(@"M=0");
            linesOfCode.Add(@"@ENDLT_" + this.lt_count);
            linesOfCode.Add(@"0;JMP");

            linesOfCode.Add(@"(LTTRUE_" + this.lt_count + ")");
            linesOfCode.Add(@"@R15");
            linesOfCode.Add(@"M=-1");

            linesOfCode.Add(@"(ENDLT_" + this.lt_count + ")");

            this.PushR("R15");

            this.lt_count++;
        }

        // x = y
        private void WriteEquality()
        {
            this.WritePopToR("R14");
            this.WritePopToR("R15");

            // subtract and check for zero - this checks if they are equal
            linesOfCode.Add(@"@R14");
            linesOfCode.Add(@"D=M");
            linesOfCode.Add(@"@R15");
            linesOfCode.Add(@"M=M-D");
            linesOfCode.Add(@"D=M"); // copy subtraction result to D
            linesOfCode.Add(@"@EQUAL_" + this.eq_count);
            linesOfCode.Add(@"D;JEQ");//jump to EQUAL if zero

            linesOfCode.Add(@"@R15");
            linesOfCode.Add(@"M=0");// push zero onto stack for false (we push at end of method
            this.linesOfCode.Add("@END_EQUAL_" + this.eq_count);
            this.linesOfCode.Add("0;JMP");

            linesOfCode.Add(@"(EQUAL_" + this.eq_count + ")");
            linesOfCode.Add(@"@R15");
            linesOfCode.Add(@"M=-1");// push -1 onto stack for true (we push at end of method)


            this.linesOfCode.Add(@"(END_EQUAL_" + this.eq_count + ")");
            this.PushR("R15");

            this.eq_count++;

        }

        #endregion

        #region StackCommands

        /// <summary>
        /// Writes the restore segment for return.
        /// See P.163
        /// </summary>
        /// <param name="segment">The segment.</param>
        /// <param name="frameIndex">Index of the frame.</param>
        private void WriteRestoreSegmentForReturn(string segmentToRestore, int frameOffset, string currentFrameLabel)
        {
            linesOfCode.Add(@"//WriteRestore " + segmentToRestore + " SegmentForReturn");

            this.linesOfCode.Add(@"@" + frameOffset);
            this.linesOfCode.Add("D=A");
            this.linesOfCode.Add(currentFrameLabel);
            this.linesOfCode.Add("A=M-D");
            this.linesOfCode.Add("D=M");
            this.linesOfCode.Add("@" + segmentToRestore);
            this.linesOfCode.Add("M=D");
        }

        /// <summary>
        /// writes the assembly code for pushing a constant onto the stack
        /// </summary>
        /// <param name="index"></param>
        private void WritePushConstant(int index)
        {
            linesOfCode.Add(@"@" + index);
            linesOfCode.Add(@"D=A");
            linesOfCode.Add(@"@SP");
            linesOfCode.Add(@"A=M");
            linesOfCode.Add(@"M=D");
            linesOfCode.Add(@"@SP");
            linesOfCode.Add(@"M=M+1");
        }

        /// <summary>
        /// Writes the assembly code to push Temp or Pointer segments 
        /// </summary>
        /// <param name="seg"></param>
        /// <param name="index"></param>
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

            linesOfCode.Add(@"@" + address); // copy contents into D
            linesOfCode.Add(@"D=M");

            linesOfCode.Add(@"@SP"); // goto top of stack
            linesOfCode.Add(@"A=M");

            linesOfCode.Add(@"M=D"); // copy D into top of stack

            this.IncrementStackPointer();
        }

        /// <summary>
        /// Writes the assembly code to push memory segment[n]
        /// </summary>
        /// <param name="seg"></param>
        /// <param name="index"></param>
        private void WritePushSegmentIndex(string seg, int index)
        {
            //May need to write push R15 etc
            int segmentBaseAddress = this.segmentLookUpTable[seg];

            this.StoreSegmentPlusIndexPointerInR13(segmentBaseAddress, index);

            this.linesOfCode.Add(@"@R13");
            this.linesOfCode.Add(@"A=M");
            this.linesOfCode.Add(@"D=M");
            this.linesOfCode.Add(@"@SP");
            this.linesOfCode.Add(@"A=M");
            this.linesOfCode.Add(@"M=D");

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
            this.linesOfCode.Add(@"@" + R);
            this.linesOfCode.Add(@"D=M");
            this.linesOfCode.Add(@"@SP");
            this.linesOfCode.Add(@"A=M");
            this.linesOfCode.Add(@"M=D");

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

            this.linesOfCode.Add(@"@SP");//copy stack top to D
            this.linesOfCode.Add(@"A=M-1");
            this.linesOfCode.Add(@"D=M");

            this.linesOfCode.Add(@"@R13");//get pointer to seg[index] from R13
            this.linesOfCode.Add(@"A=M");
            this.linesOfCode.Add(@"M=D");//make seg[index] = contents of D (which should have value from top of stack.

            this.DecrementStackPointer();

        }

        /// <summary>
        ///  does what it says on the tin!
        ///  segmentBaseAddress + index and store in R13
        /// </summary>
        /// <param name="segmentBase"></param>
        /// <param name="index"></param>
        private void StoreSegmentPlusIndexPointerInR13(int segmentBase, int index)
        {
            this.linesOfCode.Add(@"@" + index);
            this.linesOfCode.Add(@"D=A"); // D = index 

            this.linesOfCode.Add(@"@" + segmentBase);
            this.linesOfCode.Add(@"A=M+D");// now at seg[index]
            this.linesOfCode.Add(@"D=A"); //save this pointer in @R13
            this.linesOfCode.Add(@"@R13");
            this.linesOfCode.Add(@"M=D");
        }

        /// <summary>
        /// Used to Pop to an R Register
        /// </summary>
        /// <param name="R"></param>
        private void WritePopToR(string R)
        {
            int address = this.segmentLookUpTable[R];
            linesOfCode.Add(@"@SP");
            linesOfCode.Add(@"A=M-1");
            linesOfCode.Add(@"D=M");

            linesOfCode.Add(@"@" + address);
            linesOfCode.Add(@"M=D");

            this.DecrementStackPointer();
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

            this.linesOfCode.Add("@SP");
            this.linesOfCode.Add("A=M-1"); //minus one for top value
            this.linesOfCode.Add("D=M");//copy contents to D

            this.linesOfCode.Add("@" + address);
            this.linesOfCode.Add("M=D");

            this.DecrementStackPointer();
        }

        /// <summary>
        /// Writes the push static.
        /// </summary>
        /// <param name="index">The index.</param>
        private void WritePushStatic(int index)
        {
            this.linesOfCode.Add(@"@" + this.VmFileName + "." + index);
            this.linesOfCode.Add("D=M");
            this.linesOfCode.Add(@"@SP");
            this.linesOfCode.Add(@"A=M");
            this.linesOfCode.Add(@"M=D");
            this.IncrementStackPointer();
        }

        private void WritePopStatic(int index)
        {
            this.linesOfCode.Add("@SP");
            this.linesOfCode.Add("A=M-1");
            this.linesOfCode.Add("D=M");
            this.linesOfCode.Add(@"@" + this.VmFileName + "." + index);
            this.linesOfCode.Add("M=D");

            this.DecrementStackPointer();
        }

        /// <summary>
        /// Increments the stack pointer.
        /// </summary>
        private void IncrementStackPointer()
        {
            linesOfCode.Add(@"@SP");
            linesOfCode.Add(@"M=M+1");
        }

        /// <summary>
        /// Decrements the stack pointer.                                  
        /// </summary>
        private void DecrementStackPointer()
        {
            linesOfCode.Add(@"@SP");
            linesOfCode.Add(@"M=M-1");
        }
        #endregion
    }
}
