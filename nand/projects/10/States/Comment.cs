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
            // always clear chars when instance is called
            Comment.tokenCharacters = new StringBuilder();
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
                // this.ReadMultiLineComment(tokenizer);
                //TODO BRIAN start here!!!!!
            }
            else if (peekedChar == '/')
            {
                // this.ReadSingleLineComment(tokenizer);
            }

            this.ChangeState(tokenizer);

        }

        private void ReadMultiLineComment(StreamReader streamReader)
        {
            
            //while (streamReader.EndOfStream)
            //{
            //    if(streamReader.Read
            //}
        }


        private void ChangeState(ITokenizer tokenizer)
        {
            IState newToken = NewToken.Instance();
            newToken.Read(tokenizer);
        }


        #endregion
    }
}
