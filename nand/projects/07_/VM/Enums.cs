﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VM
{
    public enum CommandType
    {
        C_ARITHMETIC, C_PUSH, C_POP, C_LABEL, C_GOTO, C_IF, C_FUNCTION, C_RETURN, C_CALL, ERROR
    }
}
