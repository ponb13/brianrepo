using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Compiler
{
    public class VmWriter : IDisposable 
    {
        private StreamWriter _streamWriter;
        
        public VmWriter(string outputPath)
        {
            _streamWriter = new StreamWriter(File.Create(outputPath));
        }

        public void WritePush(Segment segment, int index)
        {
            _streamWriter.WriteLine("push " + segment.ToString().ToLower() +" "+ index);
        }

        public void WritePop(Segment segment, int index)
        {
            _streamWriter.WriteLine("pop " + segment.ToString().ToLower() + " " + index);
        }

        public void WriteArithmetic(ArithmeticCommand arithmeticCommand)
        {
            _streamWriter.WriteLine(arithmeticCommand.ToString().ToLower());
        }

        public void WriteLabel(string label)
        {
            _streamWriter.WriteLine("label " +label);
        }

        public void WriteGoto(string label)
        {
            _streamWriter.WriteLine("goto " + label);
        }

        public void WriteIf(string label)
        {
            _streamWriter.WriteLine("if-goto " + label);
        }

        public void WriteCall(string name, int numberOfArgs)
        {
            _streamWriter.WriteLine("call " + name +" "+ numberOfArgs);
        }

        public void WriteFunction(string name, int numberOfLocals)
        {
            _streamWriter.WriteLine("function " + name + " " + numberOfLocals);
        }

        public void WriteReturn()
        {
            _streamWriter.WriteLine("return");
        }

        public void Dispose()
        {
            _streamWriter.Flush();
            _streamWriter.Dispose();
        }
    }
}
