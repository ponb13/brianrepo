using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Interfaces
{
    public interface IState
    {
        /// <summary>
        /// Read from the stream / process the stream.
        /// </summary>
        /// <param name="tokenizer"></param>
        void Read(ITokenizer tokenizer);

        /// <summary>
        /// Gets or sets the token characters.
        /// The chacters that have been read for this token far.
        /// </summary>
        /// <value>The token characters.</value>
        StringBuilder TokenCharacters
        {
            get;
            set;
        }

    }
}
