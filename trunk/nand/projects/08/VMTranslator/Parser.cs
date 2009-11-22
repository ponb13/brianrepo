using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace VMTranslator
{
    /// <summary>
    /// Parses a supplied vm language file
    /// </summary>
    public class Parser : IDisposable
    {
        #region private variables
        private StreamReader reader;
        private Stream inputStream;
        /// <summary>
        /// the line from the vm file that is currently being processed 
        /// </summary>
        private string currentLine;
        #endregion

        #region ctors
        /// <summary>
        /// Initializes a new instance of the <see cref="Parser"/> class.
        /// Used only for test purposes - TODO remove!
        /// </summary>
        /// <param name="line">The line.</param>
        /// <param name="test">if set to <c>true</c> [test].</param>
        public Parser(string line, bool test)
        {
            currentLine = line;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Parser"/> class.
        /// </summary>
        /// <param name="stream">The stream.</param>
        public Parser(Stream stream)
        {
            this.inputStream = stream;
            this.reader = new StreamReader(this.inputStream);
        }



        /// <summary>
        /// Initializes a new instance of the <see cref="Parser"/> class.
        /// </summary>
        /// <param name="filePath">The file path.</param>
        public Parser(string filePath)
        {
            this.inputStream = new FileStream(filePath, FileMode.Open, FileAccess.Read);
            this.reader = new StreamReader(inputStream);
        }

        #endregion

        #region public methods
        /// <summary>
        /// Determines whether the loaded vm file has more commands.
        /// </summary>
        /// <returns>
        /// 	<c>true</c> if [has more commands]; otherwise, <c>false</c>.
        /// </returns>
        public bool HasMoreCommands()
        {
            return !this.reader.EndOfStream;
        }

        /// <summary>
        /// Read the next line in the vm file
        /// </summary>
        public void Advance()
        {
            this.currentLine = this.reader.ReadLine();
        }

        /// <summary>
        /// Gets the command type of the current line in the vm file
        /// </summary>
        /// <returns></returns>
        public CommandType GetCommandType()
        {
            // TODO not all command types implemented yet.
            CommandType commandType = CommandType.ERROR;

            if (this.IsArithmeticCommand())
            {
                commandType = CommandType.C_ARITHMETIC;
            }
            else if (this.IsGoToCommand())
            {
                commandType = CommandType.C_GOTO;
            }
            else if (this.IsLabelCommand())
            {
                commandType = CommandType.C_LABEL;
            }
            else if (this.IsPopCommand())
            {
                commandType = CommandType.C_POP;
            }
            else if (this.IsPushCommand())
            {
                commandType = CommandType.C_PUSH;
            }

            else if (this.IsIfCommand())
            {
                commandType = CommandType.C_IF;
            }
            else if (this.IsFunctionCommand())
            {
                commandType = CommandType.C_FUNCTION;
            }
            else if (this.IsReturnCommand())
            {
                commandType = CommandType.C_RETURN;
            }
            else if (this.IsCallCommand())
            {
                commandType = CommandType.C_CALL;
            }

            return commandType;
        }

        /// <summary>
        /// Gets the first supplied argument for the current vm command
        /// </summary>
        /// <returns></returns>
        public string GetArg1()
        {
            string result;

            if (this.IsReturnCommand())
            {
                throw new Exception("GetArg1 should not be called when current command is a return command");
            }
            else if (this.IsArithmeticCommand())
            {
                // just return the command itself e.g. add,sub etc
                string[] wordArray = this.currentLine.Split(' ');
                result = wordArray[0];
            }
            else
            {
                //get the first arg
                string[] wordArray = this.currentLine.Split(' ');
                result = wordArray[1];

            }
            return result;
        }

        /// <summary>
        /// Gets the second supplied argument for the current vm command.
        /// </summary>
        /// <returns></returns>
        public string GetArg2()
        {
            string result = string.Empty;
            if (this.GetCommandType() == CommandType.C_PUSH
                || this.GetCommandType() == CommandType.C_POP
                || this.GetCommandType() == CommandType.C_CALL
                || this.GetCommandType() == CommandType.C_LABEL
                || this.GetCommandType() == CommandType.C_IF
                || this.GetCommandType() == CommandType.C_FUNCTION
                || this.GetCommandType() == CommandType.C_GOTO
                )
            {
                string[] wordArray = this.currentLine.Split(' ');
                result = wordArray[2];
            }
            else
            {
                throw new Exception("GetArg2 should be called when command type is: " + this.GetCommandType());
            }

            return result;
        }
        #endregion

        #region CommandType Methods

        /// <summary>
        /// Determines whether the current command is an arithmetic command.
        /// </summary>
        /// <returns>
        /// 	<c>true</c> if [is arithmetic command]; otherwise, <c>false</c>.
        /// </returns>
        private bool IsArithmeticCommand()
        {
            bool result = false;
            if (this.currentLine.StartsWith("sub") ||
                this.currentLine.StartsWith("add") ||
                this.currentLine.StartsWith("neg") ||
                this.currentLine.StartsWith("eq") ||
                this.currentLine.StartsWith("gt") ||
                this.currentLine.StartsWith("lt") ||
                this.currentLine.StartsWith("and") ||
                this.currentLine.StartsWith("or") ||
                this.currentLine.StartsWith("not"))
            {
                result = true;
            }

            return result;
        }

        /// <summary>
        /// Determines whether the current command is a push stack command.
        /// </summary>
        /// <returns>
        /// 	<c>true</c> if [is push command]; otherwise, <c>false</c>.
        /// </returns>
        private bool IsPushCommand()
        {
            return this.currentLine.StartsWith("push");
        }

        /// <summary>
        /// Determines whether the current command is a pop stack command.
        /// </summary>
        /// <returns>
        /// 	<c>true</c> if [is pop command]; otherwise, <c>false</c>.
        /// </returns>
        private bool IsPopCommand()
        {
            return this.currentLine.StartsWith("pop");
        }

        /// <summary>
        /// Determines whether the current command is a label command.
        /// </summary>
        /// <returns>
        /// 	<c>true</c> if [is label command]; otherwise, <c>false</c>.
        /// </returns>
        private bool IsLabelCommand()
        {
            return this.currentLine.StartsWith("label");
        }

        /// <summary>
        /// Determines whether the current command is a go to statment.
        /// </summary>
        /// <returns>
        /// 	<c>true</c> if [is go to command]; otherwise, <c>false</c>.
        /// </returns>
        private bool IsGoToCommand()
        {
            return this.currentLine.StartsWith("goto");
        }

        /// <summary>
        /// Determines whether the current command is return command.
        /// </summary>
        /// <returns>
        /// 	<c>true</c> if [is return command]; otherwise, <c>false</c>.
        /// </returns>
        private bool IsReturnCommand()
        {
            return this.currentLine.StartsWith("return");
        }

        /// <summary>
        /// Determines whether the current command is a if-statment.
        /// </summary>
        /// <returns>
        /// 	<c>true</c> if [is if command]; otherwise, <c>false</c>.
        /// </returns>
        private bool IsIfCommand()
        {
            return this.currentLine.StartsWith("if-goto");
        }

        /// <summary>
        /// Determines whether the current command is a function call.
        /// </summary>
        /// <returns>
        /// 	<c>true</c> if [is function command]; otherwise, <c>false</c>.
        /// </returns>
        private bool IsFunctionCommand()
        {
            return this.currentLine.StartsWith("function");
        }

        /// <summary>
        /// Determines whether the current command is call a function call.
        /// </summary>
        /// <returns>
        /// 	<c>true</c> if [is call command]; otherwise, <c>false</c>.
        /// </returns>
        private bool IsCallCommand()
        {
            return this.currentLine.StartsWith("call");
        }

        #endregion

        #region IDisposable Members

        void IDisposable.Dispose()
        {
            inputStream.Dispose();
            this.reader.Dispose();
        }

        #endregion
    }
}
