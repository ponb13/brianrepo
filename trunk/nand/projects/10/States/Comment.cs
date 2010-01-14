using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Interfaces;
using System.IO;

namespace States
{
    public class Comment : IState
    {
        #region singleton logic
        private static IState state = new Comment();
        private static StringBuilder tokenCharacters;

        private Comment()
        {
        }

        public static IState Instance()
        {
            Comment.tokenCharacters = new StringBuilder();
            return state;
        }

        /// <summary>
        /// Creates an instance passing in chars parsed from previous state
        /// </summary>
        /// <param name="parsedCharacters">The parsed characters.</param>
        /// <returns></returns>
        public static IState Instance(StringBuilder parsedCharacters)
        {
            Comment.tokenCharacters = new StringBuilder();
            state.TokenCharacters = parsedCharacters;
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
            get { return Comment.tokenCharacters; }
            set { Comment.tokenCharacters = value; }
        }

        public void Read(ITokenizer tokenizer)
        {
            char peekedChar = (char)tokenizer.StrmReader.Peek();
            if (peekedChar == '*')
            {
                this.ReadMultiLineComment(tokenizer.StrmReader);
                
            }
            else if (peekedChar == '/')
            {
                this.ReadSingleLineComment(tokenizer.StrmReader);
            }

            tokenizer.Tokens.Add(this.CreateTokenObject());
            this.ChangeState(tokenizer);

        }

        private void ReadMultiLineComment(StreamReader streamReader)
        {
            // consumer the next opening comment char - don't confuse the code below...
            if (!streamReader.EndOfStream)
            {
                this.TokenCharacters.Append((char)streamReader.Read());
            }
            
            while (!streamReader.EndOfStream)
            {
                char readChar = (char)streamReader.Read();
                this.TokenCharacters.Append(readChar);

                // look for closing * and the closing single \
                if (readChar == '*' && (char)streamReader.Peek() == '/')
                {
                    this.TokenCharacters.Append((char)streamReader.Read());
                    break;
                }
            }
        }

        private void ReadSingleLineComment(StreamReader streamReader)
        {
            //consume the second slash of the comment openener
            if (!streamReader.EndOfStream)
            {
                this.TokenCharacters.Append((char)streamReader.Read());
            }

            while (!streamReader.EndOfStream && !this.IsEndOfLine((char)streamReader.Peek()))
            {
                this.TokenCharacters.Append((char)streamReader.Read());
            }
        }

        /// <summary>
        /// Determines whether [is end of line] [the specified read char1].
        /// </summary>
        /// <param name="readChar1">The read char1.</param>
        /// <param name="readChar2">The read char2.</param>
        /// <returns>
        /// 	<c>true</c> if [is end of line] [the specified read char1]; otherwise, <c>false</c>.
        /// </returns>
        private bool IsEndOfLine(char peekedChar)
        {
            bool isEndOfLine = false;
            if (peekedChar == '\r' || peekedChar =='\n') 
            {
                isEndOfLine = true;
            }
            return isEndOfLine;
        }


        private void ChangeState(ITokenizer tokenizer)
        {
            IState newToken = NewToken.Instance();
            newToken.Read(tokenizer);
        }


        #endregion
    }
}
