using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace VM
{
    public class Parser : IDisposable
    {
        private StreamReader reader;
        private Stream inputStream;
        public string currentLine;

        public Parser(string line, bool test)
        {
            currentLine = line;
        }
        
        public Parser(Stream stream)
        {
            this.inputStream = stream;
            this.reader = new StreamReader(this.inputStream);
        }

        public Parser(string filePath)
        {
            this.inputStream = new FileStream(filePath, FileMode.Open);
            this.reader = new StreamReader(inputStream);
        }

        public bool HasMoreCommands()
        {
            return !this.reader.EndOfStream;
        }

        public void Advance()
        {
            this.currentLine = this.reader.ReadLine();
        }

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

            return commandType;
        }

        public string GetArg1()
        {
            string result;

            if(this.IsReturnCommand())
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

        public string GetArg2()
        {
            string result = string.Empty;
            if (this.GetCommandType() == CommandType.C_PUSH
                || this.GetCommandType() == CommandType.C_POP
                || this.GetCommandType() == CommandType.C_FUNCTION
                || this.GetCommandType() == CommandType.C_CALL)
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

        #region CommandType Methods

        private bool IsArithmeticCommand()
        {
            bool result =  false;
            if (this.currentLine.Equals("sub") ||
                this.currentLine.Equals("add") ||
                this.currentLine.Equals("neg") ||
                this.currentLine.Equals("eq")  ||
                this.currentLine.Equals("gt")  ||
                this.currentLine.Equals("lt")  ||
                this.currentLine.Equals("and") ||
                this.currentLine.Equals("or")  ||
                this.currentLine.Equals("not"))
            {
                result =  true;
            }

            return result;
        }

        private bool IsPushCommand()
        {
            return this.currentLine.StartsWith("push");
        }

        private bool IsPopCommand()
        {
            return this.currentLine.StartsWith("pop");
        }
        
        private bool IsLabelCommand()
        {
            return this.currentLine.StartsWith("label");
        }

        private bool IsGoToCommand()
        {
            return this.currentLine.StartsWith("goto");
        }

        private bool IsReturnCommand()
        {
            return this.currentLine.StartsWith("return");
        }

        #endregion

        #region IDisposable Members

        void IDisposable.Dispose()
        {
            inputStream.Flush();
            this.reader.Close();
        }

        #endregion
    }
}
