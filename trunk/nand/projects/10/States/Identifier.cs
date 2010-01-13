using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Interfaces;
using System.IO;
using System.Text.RegularExpressions;

namespace States 
{
    public class Identifier : IState
    {
        private static StringBuilder tokenCharacters;
        
        #region singleton logic
        private static IState state = new Identifier();

        private Identifier()
        {

        }

        /// <summary>
        /// Create an instance with chars already parsed
        /// i.e. move to this state but give chars from previous state
        /// </summary>
        /// <param name="parsedCharacters"></param>
        /// <returns></returns>
        public static IState Instance(StringBuilder parsedCharacters)
        {
            Identifier.tokenCharacters = new StringBuilder();            
            state.TokenCharacters = parsedCharacters;
            return state;
        }

        /// <summary>
        /// Gets the sinlge instance
        /// </summary>
        /// <returns></returns>
        public static IState Instance()
        {
            // always clear chars when instance is called
            state.TokenCharacters = new StringBuilder();
            return state;
        }
        #endregion

        #region IState Members
        /// <summary>
        /// Gets or sets the token characters.
        /// </summary>
        /// <value>The token characters.</value>
        public StringBuilder TokenCharacters
        {
            get { return Identifier.tokenCharacters; }
            set { Identifier.tokenCharacters = value; }
        }

        /// <summary>
        /// Reads from the stream 
        /// </summary>
        /// <param name="tokenizer">The tokenizer.</param>
        public void Read(ITokenizer tokenizer)
        {
            StreamReader streamReader = tokenizer.StrmReader;

            while (this.IsValidIdentifierCharacter((char)streamReader.Peek()))
            {
                this.TokenCharacters.Append((char)streamReader.Read());
            }

            tokenizer.Tokens.Add(this.CreateTokenObject());

            this.ChangeState(tokenizer);
        }

        /// <summary>
        /// Changes the state.
        /// </summary>
        /// <param name="tokenizer">The tokenizer.</param>
        private void ChangeState(ITokenizer tokenizer)
        {
            if (this.ReadCharsAreValidIdentifier(this.TokenCharacters.ToString()))
            {
                IState nextState = NewToken.Instance();
                nextState.Read(tokenizer);

            }
            else
            {
                throw new Exception("Invalid identifier");
            }
        }

        /// <summary>
        /// Determines if single char is valid as part of a Identifier
        /// alphanumeric and underscore are valid characters
        /// </summary>
        /// <param name="peekedChar"></param>
        /// <returns></returns>
        private bool IsValidIdentifierCharacter(char peekedChar)
        {
            string validIdentifierCharPattern = "[0-9a-zA-Z_]";
            Match match = Regex.Match(peekedChar.ToString(), validIdentifierCharPattern, RegexOptions.Compiled);

            return match.Success;
        }

        /// <summary>
        /// Determines wether the current token is a valid Identier.
        /// </summary>
        /// <param name="tokenCharacters">The token characters.</param>
        /// <returns></returns>
        private bool ReadCharsAreValidIdentifier(string tokenCharacters)
        {
            //  TODO need to ensure not a keyword aswell? might not need to as we should 
            //  only transition to this state from keyowrd state

            // TODO need to ensure does not start with digit
            string validIdentifierPattern = "[0-9a-zA-Z_]*";
            Match match = Regex.Match(tokenCharacters, validIdentifierPattern, RegexOptions.Compiled);

            return match.Success;
        }

        #endregion
    }
}
