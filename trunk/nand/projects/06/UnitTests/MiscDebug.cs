using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Assembler;

namespace UnitTests
{
    /// <summary>
    /// Summary description for Debug
    /// </summary>
    [TestClass]
    public class MiscDebug
    {
        public MiscDebug()
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
        public void SomeTest()
        {
            Parser parser = new Parser();
            parser.currentTxtCommand = "D=A";

            String destMnemonic = parser.Dest();
            Assert.IsTrue(destMnemonic == "D", "parser failed");

            String compMnemonic = parser.Comp();
            Assert.IsTrue(compMnemonic == "A", "parser failed");

            string destBin = CodeGenerator.Dest(destMnemonic);
            Assert.IsTrue(destBin == "010");

            string compbin = CodeGenerator.Comp(compMnemonic);
            Assert.IsTrue(compbin == "0110000");



        }
    }
}
