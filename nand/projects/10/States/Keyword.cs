using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Interfaces;
using System.IO;
using System.Text.RegularExpressions;

namespace States
{
    public class Keyword : IState
    {
        #region singleton logic
        private static IState state = new Keyword();
        private static StringBuilder tokenCharacters;

        private Keyword()
        {
        }

        public static IState Instance()
        {
            // always clear chars when instance is called
            Keyword.tokenCharacters = new StringBuilder();
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
            get{ return Keyword.tokenCharacters; }
            set{ Keyword.tokenCharacters = value; }
        }
        
        public void Read(ITokenizer tokenizer)
        {
            StreamReader streamReader = tokenizer.StrmReader;
            while (this.IsValidKeywordChar((char)streamReader.Peek()))
            {
                this.TokenCharacters.Append((char)streamReader.Read());
            }

            this.ChangeState(tokenizer);
        }

        /// <summary>
        /// Changes the state of the ITokenizer instance.
        /// </summary>
        /// <param name="tokenizer">The tokenizer.</param>
        private void ChangeState(ITokenizer tokenizer)
        {
            if (this.ReadCharsAreValidKeyword(tokenizer))
            {
                tokenizer.Tokens.Add(this.CreateTokenObject());
                tokenizer.State = NewToken.Instance();
            }
            else 
            {
                tokenizer.State = Identifier.Instance(this.TokenCharacters);
            }

            tokenizer.State.Read(tokenizer);
        }

        /// <summary>
        /// Determines whether a single char is a valid keyword character.
        /// i.e. only lower case letters are valid keyword characters.
        /// </summary>
        /// <param name="peekedChar">The peeked char.</param>
        /// <returns>
        /// 	<c>true</c> if [is keyword end char] [the specified peeked char]; otherwise, <c>false</c>.
        /// </returns>
        private bool IsValidKeywordChar(char peekedChar)
        {
            string validKeyWordCharacter = "[a-z]";
            Match match = Regex.Match(peekedChar.ToString(), validKeyWordCharacter, RegexOptions.Compiled);

            return match.Success;
        }

        /// <summary>
        /// Determines if the chars read so far constitute a valid keyword.
        /// </summary>
        /// <param name="tokenizer">The tokenizer.</param>
        /// <returns></returns>
        private bool ReadCharsAreValidKeyword(ITokenizer tokenizer)
        {
            string keywordPattern = "class|constructor|function|method|field|static|var|int|char|boolean|void|true|false|null|this|let|do|if|else|while|return";
            Match match = Regex.Match(this.TokenCharacters.ToString(), keywordPattern, RegexOptions.Compiled);

            return match.Success;
        }

        #endregion
    }
}
