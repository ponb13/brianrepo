 using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assembler
{
    /// <summary>
    /// All the possible command types that can be found 
    /// in the hack platform assembly language
    /// (except error!!)
    /// </summary>
    public enum Command { A_COMMAND, C_COMMAND, L_COMMAND, ERROR };
}
