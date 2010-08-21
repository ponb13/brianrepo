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

    public enum Segment
    {
        None, Constant, Arguement, Local, Static, This, That, Pointer, Temp
    }

    public enum ArithmeticCommand
    {
        Divide, Mult, Add, Sub, Neg, Eq, Gt, Lt, And, Or, Not
    }

    public enum Scope
    {
        ClassLevel, MethodLevel
    }
}
