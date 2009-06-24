using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Assembler
{
    public class Parser: IDisposable
    {
        private StreamReader streamReader;

        public enum Command { A_COMMAND, C_COMMAND, L_COMMAND, ERROR };

        public string currentTxtCommand;
        
        public Parser(string filePath)
        {
            streamReader = new StreamReader(filePath);
        }

        public bool HasMoreCommands()
        {
            return !streamReader.EndOfStream;
        }

        public void Advance()
        {
            if (this.HasMoreCommands())
            {
                this.currentTxtCommand = streamReader.ReadLine();
            }
        }

        public Command CommandType
        {
            get 
            {
                return GetCommandType();
            }
        }

        private Command GetCommandType()
        {
            // currently doesn't handle symbols (L commands see p110)
            Command commandType = Command.ERROR;
            
            if (this.currentTxtCommand.StartsWith("@")) // if it starts with @
            {
                commandType = Command.A_COMMAND;
            }
            else if(!this.currentTxtCommand.Contains('/') // if not start with comnment and is not empty
                && !String.IsNullOrEmpty(this.currentTxtCommand))
            {
                commandType = Command.C_COMMAND;
            }                                

            return commandType;
        }


        #region IDisposable Members

        public void Dispose()
        {
            if (streamReader != null)
            {
                streamReader.Close();
            }
        }

        #endregion
    }
}
