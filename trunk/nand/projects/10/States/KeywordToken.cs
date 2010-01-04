using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Interfaces;
using System.IO;
using System.Text.RegularExpressions;

namespace States
{
    public class KeywordToken : IState
    {
        #region singleton logic
        private static IState state = new KeywordToken();

        private KeywordToken()
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
            while (this.IsValidKeywordChar((char)streamReader.Peek()))
            {
                tokenizer.TokenCharacters.Append((char)streamReader.Read());
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
                tokenizer.State = TokenComplete.Instance();
            }
            else 
            {
                tokenizer.State = IdentifierToken.Instance();
            }
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
            Match match = Regex.Match(tokenizer.TokenCharacters.ToString(), keywordPattern, RegexOptions.Compiled);

            return match.Success;
        }

        #endregion
    }
}
