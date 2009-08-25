using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace VM
{
    public class Parser : IDisposable
    {
        StreamReader reader;
        Stream inputStream;
        string currentLine;
        
        public Parser(Stream stream)
        {
            this.inputStream = stream;
            this.reader = new StreamReader(this.inputStream);
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
            // TODO
            return CommandType.ERROR;
        }

        public string GetArg1()
        {
            // TODO
            return "ERROR";
        }

        public string GetArg2()
        {
            //TODO
            return "ERROR";
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
            bool result = false;

            if(this.currentLine.StartsWith("push"))
            {
                result = true;
            }

            return result;
        }

        private bool IsPopCommand()
        {
            bool result = false;

            if(this.currentLine.StartsWith("pop"))
            {
                result = true;
            }

            return result;
        }

        private bool 

        #endregion





        #region IDisposable Members

        void IDisposable.Dispose()
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
