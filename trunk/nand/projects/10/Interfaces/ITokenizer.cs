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

        StringBuilder TokenCharacters
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
