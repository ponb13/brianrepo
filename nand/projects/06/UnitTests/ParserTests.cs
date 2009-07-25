using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Assembler;

using System.IO;

namespace UnitTests
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
        /// Tests that comp 
        /// </summary>
        [TestMethod]
        public void CompTest()
        {
            Parser parser = new Parser();

            parser.currentTxtCommand = "D=A";
            parser.Comp();
            Assert.IsTrue(parser.Comp() == "A");
 
            parser.currentTxtCommand = "M=D";
            parser.Comp();
            Assert.IsTrue(parser.Comp() == "D");

            parser.currentTxtCommand = ";JMP";
            parser.Comp();
            Assert.IsTrue(parser.Comp() == String.Empty);

            parser.currentTxtCommand = "0;JMP";
            parser.Comp();
            Assert.IsTrue(parser.Comp() == "0");

            parser.currentTxtCommand = "M=D+M";
            parser.Comp();
            Assert.IsTrue(parser.Comp() == "D+M");

            parser.currentTxtCommand = "MD=M+1";
            parser.Comp();
            Assert.IsTrue(parser.Comp() == "M+1");

            parser.currentTxtCommand = "D;JNE";
            parser.Comp();
            Assert.IsTrue(parser.Comp() == "D");

            bool exceptionThrown = false;
            try
            {
                parser.currentTxtCommand = "@0";
                parser.Comp();
            }
            catch
            {
                exceptionThrown = true;
            }
            Assert.IsTrue(exceptionThrown);
        }
    }
}
