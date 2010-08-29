using System;
using System.IO;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Compiler;

namespace Compiler.Tests
{
    [TestClass]
    public class VmWriterTests
    {
        [TestMethod]
        public void ConstructorTest()
        {
            string outputPath = "../../../TestFiles/PrintNumber/UnitTest.vm";
            VmWriter vmWriter = new VmWriter(outputPath);

            Assert.IsTrue(File.Exists(outputPath), "file not found.");
        }

        [TestMethod]
        public void WritePushTest()
        {
            string outputPath = "../../../TestFiles/PrintNumber/UnitTest.vm";
            using(VmWriter vmWriter = new VmWriter(outputPath))
            {
                vmWriter.WritePush(Segment.Argument, 1);
            }

            using (StreamReader streamReader = new StreamReader(File.OpenRead("../../../TestFiles/PrintNumber/UnitTest.vm")))
            {
                string line = streamReader.ReadLine().Trim();
                Assert.IsTrue(string.Equals(line, "push Argument 1", StringComparison.InvariantCulture));
            }
        }

        [TestMethod]
        public void WritePopTest()
        {
            string outputPath = "../../../TestFiles/PrintNumber/UnitTest.vm";
            using (VmWriter vmWriter = new VmWriter(outputPath))
            {
                vmWriter.WritePop(Segment.Argument, 1);
            }

            using (StreamReader streamReader = new StreamReader(File.OpenRead("../../../TestFiles/PrintNumber/UnitTest.vm")))
            {
                string line = streamReader.ReadLine().Trim();
                Assert.IsTrue(string.Equals(line, "pop Argument 1", StringComparison.InvariantCulture));
            }
        }

        [TestMethod]
        public void WriteArithmeticTest()
        {
            string outputPath = "../../../TestFiles/PrintNumber/UnitTest.vm";
            using (VmWriter vmWriter = new VmWriter(outputPath))
            {
                vmWriter.WriteArithmetic(ArithmeticCommand.Add);
            }

            using (StreamReader streamReader = new StreamReader(File.OpenRead("../../../TestFiles/PrintNumber/UnitTest.vm")))
            {
                string line = streamReader.ReadLine().Trim();
                Assert.IsTrue(string.Equals(line, "add", StringComparison.InvariantCulture));
            }
        }

        [TestMethod]
        public void WriteLabelTest()
        {
            string outputPath = "../../../TestFiles/PrintNumber/UnitTest.vm";
            using (VmWriter vmWriter = new VmWriter(outputPath))
            {
                vmWriter.WriteLabel("SomeLabel");
            }

            using (StreamReader streamReader = new StreamReader(File.OpenRead("../../../TestFiles/PrintNumber/UnitTest.vm")))
            {
                string line = streamReader.ReadLine().Trim();
                Assert.IsTrue(string.Equals(line, "label SomeLabel", StringComparison.InvariantCulture));
            }
        }

        [TestMethod]
        public void WriteGotoTest()
        {
            string outputPath = "../../../TestFiles/PrintNumber/UnitTest.vm";
            using (VmWriter vmWriter = new VmWriter(outputPath))
            {
                vmWriter.WriteGoto("SomeLabel");
            }

            using (StreamReader streamReader = new StreamReader(File.OpenRead("../../../TestFiles/PrintNumber/UnitTest.vm")))
            {
                string line = streamReader.ReadLine().Trim();
                Assert.IsTrue(string.Equals(line, "goto SomeLabel", StringComparison.InvariantCulture));
            }
        }

        [TestMethod]
        public void WriteIfTest()
        {
            string outputPath = "../../../TestFiles/PrintNumber/UnitTest.vm";
            using (VmWriter vmWriter = new VmWriter(outputPath))
            {
                vmWriter.WriteIf("SomeLabel");
            }

            using (StreamReader streamReader = new StreamReader(File.OpenRead("../../../TestFiles/PrintNumber/UnitTest.vm")))
            {
                string line = streamReader.ReadLine().Trim();
                Assert.IsTrue(string.Equals(line, "if-goto SomeLabel", StringComparison.InvariantCulture));
            }
        }

        [TestMethod]
        public void WriteCallTest()
        {
            string outputPath = "../../../TestFiles/PrintNumber/UnitTest.vm";
            using (VmWriter vmWriter = new VmWriter(outputPath))
            {
                vmWriter.WriteCall("SomeFunction", 5);
            }

            using (StreamReader streamReader = new StreamReader(File.OpenRead("../../../TestFiles/PrintNumber/UnitTest.vm")))
            {
                string line = streamReader.ReadLine().Trim();
                Assert.IsTrue(string.Equals(line, "call SomeFunction 5", StringComparison.InvariantCulture));
            }
        }

        [TestMethod]
        public void WriteFunctionTest()
        {
            string outputPath = "../../../TestFiles/PrintNumber/UnitTest.vm";
            using (VmWriter vmWriter = new VmWriter(outputPath))
            {
                vmWriter.WriteFunction("SomeFunction", 5);
            }

            using (StreamReader streamReader = new StreamReader(File.OpenRead("../../../TestFiles/PrintNumber/UnitTest.vm")))
            {
                string line = streamReader.ReadLine().Trim();
                Assert.IsTrue(string.Equals(line, "function SomeFunction 5", StringComparison.InvariantCulture));
            }
        }

        [TestMethod]
        public void WriteReturnTest()
        {
            string outputPath = "../../../TestFiles/PrintNumber/UnitTest.vm";
            using (VmWriter vmWriter = new VmWriter(outputPath))
            {
                vmWriter.WriteReturn();
            }

            using (StreamReader streamReader = new StreamReader(File.OpenRead("../../../TestFiles/PrintNumber/UnitTest.vm")))
            {
                string line = streamReader.ReadLine().Trim();
                Assert.IsTrue(string.Equals(line, "return", StringComparison.InvariantCulture));
            }
        }
    }
}
