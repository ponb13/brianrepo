using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Interfaces
{
    public interface ITokenizer
    {
        IState State
        {
            get;
            set;
        }

        IList<Pair<string, string>> Tokens
        {
            get;
            set;
        }

        StreamReader StrmReader
        {
            get;
            set;
        }
        
    }
}
