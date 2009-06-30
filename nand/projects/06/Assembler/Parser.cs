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

        public Command CommandType()
        {
            // currently doesn't handle symbols (L commands see p110)
            Command commandType = Command.ERROR;
            
            if (this.currentTxtCommand.StartsWith("@")) // if it starts with @
            {
                commandType = Command.A_COMMAND;
            }
            else if(this.currentTxtCommand.StartsWith("("))
            {
                // TODO
                // this is wrong !! see label symbols spec !
                commandType = Command.L_COMMAND;
            }
            else if(this.currentTxtCommand.Contains('='))
            {
                // TODO
                // should work needs checked
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
            if (this.CommandType() != Command.C_COMMAND)
            {
                throw new Exception("Dest should only be called when CommandType is C_Command " + Environment.NewLine
                    + "Current Command Type: " + this.CommandType() + Environment.NewLine
                    + "Current current text command: " + this.currentTxtCommand);

            }

            // the binary that will be calculated in this method
            string binary = null;

            // matches and 3 uppercase letters before '=' sign
            Regex regex = new Regex(@"([A-Z]{1,3})(?==)");

            Match match = regex.Match(currentTxtCommand);

            string theDestinationNnemonic = match.Groups[1].Value;

            // note we don't check for success, could be jump command which will give null
            // see default case
            switch (theDestinationNnemonic)
            {
                case "M":
                    {
                        binary = "001";
                        break;
                    }
                case "D":
                    {
                        binary = "010";
                        break;
                    }
                case "MD":
                    {
                        binary = "011";
                        break;
                    }
                case "A":
                    {
                        binary = "100";
                        break;
                    }
                case "AM":
                    {
                        binary = "101";
                        break;
                    }
                case "AD":
                    {
                        binary = "110";
                        break;
                    }
                case "AMD":
                    {
                        binary = "111";
                        break;
                    }
                default:
                    {
                        // dest will be null on a c instruction if its a jmp instruction
                        // jmp instruction won't have a '='
                        // so if nothing is matched default to null see p110.
                        // see page 109
                        binary = "000";
                        break;
                    }
                }

            return binary;
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
