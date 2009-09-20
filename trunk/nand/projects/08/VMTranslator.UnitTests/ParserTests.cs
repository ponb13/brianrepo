using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using VMTranslator;

namespace VMTranslator.UnitTests
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

        /// <summary>
        /// Tests parser GetCommandType.
        /// </summary>
        [TestMethod]
        public void GetCommandTypeTest()
        {
            Parser parser = new Parser("sub", true);
            Assert.IsTrue(parser.GetCommandType() == CommandType.C_ARITHMETIC);

            parser = new Parser("push constant 2", true);
            Assert.IsTrue(parser.GetCommandType() == CommandType.C_PUSH);

            parser = new Parser("pop static 8", true);
            Assert.IsTrue(parser.GetCommandType() == CommandType.C_POP);

            parser = new Parser("label IF_FALSE", true);
            Assert.IsTrue(parser.GetCommandType() == CommandType.C_LABEL);

            parser = new Parser("goto IF_FALSE", true);
            Assert.IsTrue(parser.GetCommandType() == CommandType.C_GOTO);

            parser = new Parser("if-goto IF_TRUE", true);
            Assert.IsTrue(parser.GetCommandType() == CommandType.C_IF);

            parser = new Parser("function Main.fibonacci 0", true);
            Assert.IsTrue(parser.GetCommandType() == CommandType.C_FUNCTION);

            parser = new Parser("return", true);
            Assert.IsTrue(parser.GetCommandType() == CommandType.C_RETURN);

            parser = new Parser("call Main.fibonacci 1", true);
            Assert.IsTrue(parser.GetCommandType() == CommandType.C_CALL);
        }

        /// <summary>
        /// Tests that Arg 1 is parsed correctly
        /// </summary>
        [TestMethod]
        public void GetArg1and2Test()
        {
            Parser parser = new Parser("push constant 2", true);
            Assert.IsTrue(parser.GetArg1() == "constant");
            Assert.IsTrue(parser.GetArg2() == "2");

            parser = new Parser("pop static 8", true);
            Assert.IsTrue(parser.GetArg1() == "static");
            Assert.IsTrue(parser.GetArg2() == "8");

            parser = new Parser("label IF_FALSE", true);
            Assert.IsTrue(parser.GetArg1() == "IF_FALSE");

            parser = new Parser("goto IF_FALSE", true);
            Assert.IsTrue(parser.GetArg1() == "IF_FALSE");

            parser = new Parser("if-goto IF_TRUE", true);
            Assert.IsTrue(parser.GetArg1() == "IF_TRUE");

            parser = new Parser("function Main.fibonacci 0", true);
            Assert.IsTrue(parser.GetArg1() == "Main.fibonacci");
            Assert.IsTrue(parser.GetArg2() == "0");

            parser = new Parser("call Main.fibonacci 1", true);
            Assert.IsTrue(parser.GetArg1() == "Main.fibonacci");
            Assert.IsTrue(parser.GetArg2() == "1");
        }
    }
}
