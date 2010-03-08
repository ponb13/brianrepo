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
        private IDictionary<string, Identifier> classScope;

        /// <summary>
        /// stores variables in the subroutine scope
        /// </summary>
        private IDictionary<string, Identifier> subRoutineScope;

        /// <summary>
        /// Initializes a new instance of the <see cref="SymbolTable"/> class.
        /// </summary>
        public SymbolTable()
        {
            this.classScope = new Dictionary<string, Identifier>();
            this.subRoutineScope = new Dictionary<string, Identifier>(); 
        }

        /// <summary>
        /// Starts the new subroutine.
        /// i.e. resets the subroutine scope
        /// </summary>
        public void StartNewSubroutine()
        {
            this.subRoutineScope = new Dictionary<string, Identifier>();
        }

        /// <summary>
        /// Defines a new identifier
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="type">The type.</param>
        /// <param name="kind">The kind.</param>
        public void Define(string name, string type, Kind kind)
        {
            
            if (kind == Kind.Static || kind == Kind.Field)
            {
                
            }
        }

        
    }
}
