using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;

namespace VM.UnitTests
{
    /// <summary>
    /// Summary description for UnitTest1
    /// </summary>
    [TestClass]
    public class ParserTests
    {
        public ParserTests()
        {
            //
            // TODO: Add constructor logic here
            //
        }

        private TestContext testContextInstance;

        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext
        {
            get
            {
                return testContextInstance;
            }
            set
            {
                testContextInstance = value;
            }
        }

        #region Additional test attributes
        //
        // You can use the following additional attributes as you write your tests:
        //
        // Use ClassInitialize to run code before running the first test in the class
        // [ClassInitialize()]
        // public static void MyClassInitialize(TestContext testContext) { }
        //
        // Use ClassCleanup to run code after all tests in a class have run
        // [ClassCleanup()]
        // public static void MyClassCleanup() { }
        //
        // Use TestInitialize to run code before running each test 
        // [TestInitialize()]
        // public void MyTestInitialize() { }
        //
        // Use TestCleanup to run code after each test has run
        // [TestCleanup()]
        // public void MyTestCleanup() { }
        //
        #endregion

        [TestMethod]
        public void GetArg1Test()
        {
            Parser parser = new Parser("pop this 6");
            Assert.IsTrue(parser.GetArg1() == "this");

            parser = new Parser("push constant 36");
            Assert.IsTrue(parser.GetArg1() == "constant");

            parser = new Parser("sub");
            Assert.IsTrue(parser.GetArg1() == "sub");

            bool excpetionThrown = false;
            try
            {
                parser = new Parser("return");
                parser.GetArg1();
            }
            catch
            {
                excpetionThrown = true;
            }
            Assert.IsTrue(excpetionThrown);
        }

        [TestMethod]
        public void GetArg2Test()
        {
            Parser parser = new Parser("pop this 6");
            Assert.IsTrue(parser.GetArg2() == "6");

            parser = new Parser("push constant 36");
            Assert.IsTrue(parser.GetArg2() == "36");
            
            bool excpetionThrown = false;
            try
            {
                parser = new Parser("return");
                parser.GetArg2();
            }
            catch
            {
                excpetionThrown = true;
            }
            Assert.IsTrue(excpetionThrown);
        }

        [TestMethod]
        public void ParserInputFromFileTest()
        {
            using(FileStream fs = new FileStream(@"..\..\..\StackArithmetic\SimpleAdd\SimpleAdd.vm", FileMode.Open))
            using(Parser parser = new Parser(fs))
            {
                while (parser.HasMoreCommands())
                {
                    parser.Advance();
                }
            }
        }
    }
}
