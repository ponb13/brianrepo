using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Text.RegularExpressions;

namespace Assembler
{
    public class Parser: IDisposable
    {
        private StreamReader reader;

        public string currentTxtCommand;
        
        public Parser(Stream inputStream)
        {
            reader = new StreamReader(inputStream);
        }

        public bool HasMoreCommands()
        {
            return !reader.EndOfStream;
        }

        public void Advance()
        {
            if (this.HasMoreCommands())
            {
                this.currentTxtCommand = reader.ReadLine();
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
            else if(this.currentTxtCommand.StartsWith("("))
            {
                commandType = Command.L_COMMAND;
            }
            else
            {
                commandType = Command.C_COMMAND;
            }                                

            return commandType;
        }

        /// <summary>
        /// Returns the destination mnemonic of the current command
        /// Should only be called if the current command is a c command.
        /// </summary>
        /// <returns></returns>
        public string Dest()
        {
            string result = string.Empty;

            Regex regex = new Regex(@"([A-Z]{1,3})(?==)");

            Match match = regex.Match(currentTxtCommand);

            if (match.Success)
            {
                result = match.Groups[1].Value; 
            }

            return result;
        }


        #region IDisposable Members

        public void Dispose()
        {
            if (reader != null)
            {
                reader.Close();
            }
        }

        #endregion
    }
}
