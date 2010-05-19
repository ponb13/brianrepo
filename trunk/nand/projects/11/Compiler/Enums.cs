using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Compiler
{
    public enum Kind
    {
        Static, Field, Arg, Var, None
    }

    public enum IdentifierUsage
    {
        InUse, Defined
    }
}
