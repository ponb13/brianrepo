using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Interfaces
{
    public interface ITokenizer
    {
        IState State
        {
            get;
            set;
        }

        IList<char> TokenChars
        {
            get;
            set;
        }
    }
}
