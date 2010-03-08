using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Compiler
{
    public class SymbolTable
    {
        /// <summary>
        /// stores variables in the class scope.
        /// </summary>
        private IDictionary<string, SymbolTableEntry> classScope;

        /// <summary>
        /// stores variables in the subroutine scope
        /// </summary>
        private IDictionary<string, SymbolTableEntry> subRoutineScope;
        
    }
}
