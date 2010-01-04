using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Interfaces;
using System.IO;
using System.Text.RegularExpressions;

namespace States 
{
    public class IdentifierToken : IState
    {
        #region singleton logic
        private static IState state = new IdentifierToken();

        private IdentifierToken()
        {

        }

        public static IState Instance()
        {
            return state;
        }

        #endregion



        #region IState Members

        public void Read(ITokenizer tokenizer)
        {
            StreamReader streamReader = tokenizer.StrmReader;

            while (this.IsValidIdentifierCharacter((char)streamReader.Peek()))
            {
                tokenizer.TokenCharacters.Append((char)streamReader.Read());
            }

            this.ChangeState(tokenizer);
        }

        private void ChangeState(ITokenizer tokenizer)
        {
            if(this.ReadCharsAreValidIdentifier(tokenizer.TokenCharacters.ToString()))
            {
                tokenizer.State = TokenComplete.Instance();
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
            //  TODO need to ensure not a keyword aswell? might need to as we should 
            //  only transition to this state from keyowrd state

            // TODO need to ensure does not start with digit
            string validIdentifierPattern = "[0-9a-zA-Z_]*";
            Match match = Regex.Match(tokenCharacters, validIdentifierPattern, RegexOptions.Compiled);

            return match.Success;
        }

        #endregion
    }
}
