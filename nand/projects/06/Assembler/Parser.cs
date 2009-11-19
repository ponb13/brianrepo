using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Text.RegularExpressions;

namespace Assembler
{
    public class Parser : IDisposable
    {
        private StreamReader reader;

        /// <summary>
        /// the currently loaded line.
        /// </summary>
        public string currentTxtCommand;

        public Parser()
        {
            // useful for testing
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Parser"/> class.
        /// </summary>
        /// <param name="inputStream">The input stream.</param>
        public Parser(Stream inputStream)
        {
            reader = new StreamReader(inputStream);
        }

        /// <summary>
        /// Determines whether the assembly language file has more commands.
        /// </summary>
        /// <returns>
        /// 	<c>true</c> if [has more commands]; otherwise, <c>false</c>.
        /// </returns>
        public bool HasMoreCommands()
        {
            return !reader.EndOfStream;
        }

        /// <summary>
        /// Readers the next line of the currently loaded assembly language file
        /// </summary>
        public void Advance()
        {
            if (this.HasMoreCommands())
            {
                this.currentTxtCommand = reader.ReadLine();
            }
        }

        /// <summary>
        /// Finds the command type of the current assembly language line
        /// </summary>
        /// <returns></returns>
        public Command CommandType()
        {
            Command commandType = Command.ERROR;

            if (this.currentTxtCommand.StartsWith("@")) // if it starts with @
            {
                commandType = Command.A_COMMAND;
            }
            else if (this.currentTxtCommand.StartsWith("("))
            {
                // TODO check this
                commandType = Command.L_COMMAND;
            }
            // this if must be last
            else if (this.currentTxtCommand.Contains('=')
                || this.currentTxtCommand.Contains(";"))
            {
                // TODO - should probably use regex - see P.109 for spec of c command
                commandType = Command.C_COMMAND;
            }

            return commandType;
        }

        /// <summary>
        /// Extracts the destination Mnemonic from the assembly language line
        /// Should only be called if the current command is a c command.
        /// </summary>
        /// <returns></returns>
        public string Dest()
        {
            string destination = string.Empty;
            
            if (this.CommandType() != Command.C_COMMAND)
            {
                throw new Exception("Dest should only be called when CommandType is C_Command " + Environment.NewLine
                    + "Current Command Type: " + this.CommandType() + Environment.NewLine
                    + "Current current text command: " + this.currentTxtCommand);

            }

            destination = this.ExtractMnemonic(@"([A-Z]{1,3})(?==)", 1);

            return destination;
        }

        /// <summary>
        /// Returns the symbol in an A_Command or L_Command
        /// </summary>
        /// <returns></returns>
        public string Symbol()
        {
            // if not a L_Command or a C_command
            if (this.CommandType() == Command.C_COMMAND ) 
            {
                throw new Exception("Symbol should only be called when CommandType is L_Command or A_command " + Environment.NewLine
                    + "Current Command Type: " + this.CommandType() + Environment.NewLine
                    + "Current current text command: " + this.currentTxtCommand);

            }
            
            string symbol = string.Empty;

            // match after an @
            symbol = this.ExtractMnemonic(@"(@)([^\r\n]*)", 2);

            if (String.IsNullOrEmpty(symbol))
            {
                // extracts between brackets
                symbol = this.ExtractMnemonic(@"(\()([^\r\n]*)(\))", 2);
            }

            return symbol;
        }

        /// <summary>
        /// Returns the comp part of a C instruction, see page 109.
        /// </summary>
        /// <returns></returns>
        public string Comp()
        {
            if (this.CommandType() != Command.C_COMMAND)
            {
                throw new Exception("Comp should only be called when CommandType is C_Command " + Environment.NewLine
                    + "Current Command Type: " + this.CommandType() + Environment.NewLine
                    + "Current current text command: " + this.currentTxtCommand);

            }
            
            string comp = String.Empty;

            // matches after '='
            comp = this.ExtractMnemonic(@"(=)([^\r\n]*)", 2);

            if (String.IsNullOrEmpty(comp))
            {
                // matches before ';'
                comp = this.ExtractMnemonic(@"([^\r\n]*)(;)", 1);
            }
           
            return comp;
        }

        /// <summary>
        /// Extracts the jump Mnemonic from the assembly language line
        /// </summary>
        /// <returns></returns>
        public string Jump()
        {
            if (this.CommandType() != Command.C_COMMAND)
            {
                throw new Exception("Jump should only be called when CommandType is C_Command " + Environment.NewLine
                    + "Current Command Type: " + this.CommandType() + Environment.NewLine
                    + "Current current text command: " + this.currentTxtCommand);

            }
            
            string jump = String.Empty;

            jump = this.ExtractMnemonic(@"(;)([^\r\n]*)",2);

            return jump;
        }

        /// <summary>
        /// Extracts the Mnemonic from the current line (currentTxtCommand)
        /// </summary>
        /// <param name="regexPattern"></param>
        /// <param name="groupNumber"></param>
        /// <returns></returns>
        private string ExtractMnemonic(String regexPattern, int groupNumber)
        {
            string result = String.Empty;

            Regex regex = new Regex(regexPattern);
            Match match = regex.Match(this.currentTxtCommand);

            if (match.Success)
            {
                result = match.Groups[groupNumber].Value;
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
