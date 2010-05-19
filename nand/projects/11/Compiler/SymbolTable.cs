using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Compiler
{
    public class SymbolTable
    {
        /// <summary>
        /// stores identifiers in the class scope.
        /// </summary>
        private IDictionary<string, Identifier> classScope;

        /// <summary>
        /// stores identifiers in the subroutine scope
        /// </summary>
        private IDictionary<string, Identifier> subRoutineScope;

        /// <summary>
        /// Static, Field, Arg, Var, keep running index
        /// Index is zero based, hack start at -1 
        /// so first increment sets to zero
        /// </summary>
        private int staticCount = 0;
        private int fieldCount = 0;
        private int argCount = 0;
        private int varCount = 0;

        private int classScopeIdentifierIndex = 0;
        private int subRoutineScopeIdentifierIndex = 0;

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
            this.ResetSubRoutineVarCountsIndex();

            this.subRoutineScope = new Dictionary<string, Identifier>();
        }

        /// <summary>
        /// Defines a new identifier
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="type">The type.</param>
        /// <param name="kind">The kind.</param>
        public Identifier Define(Identifier identifier)
        {
            this.IncrementVarKindCount(identifier);

            if (identifier.Kind == Kind.Static || identifier.Kind == Kind.Field)
            {
                identifier.Index = this.classScopeIdentifierIndex;
                this.classScopeIdentifierIndex++;
                this.classScope.Add(identifier.Name, identifier);
            }
            else if (identifier.Kind == Kind.Var || identifier.Kind == Kind.Arg)
            {
                identifier.Index = this.subRoutineScopeIdentifierIndex;
                this.subRoutineScopeIdentifierIndex++;
                this.subRoutineScope.Add(identifier.Name, identifier);
            }

            return identifier;
        }

        /// <summary>
        /// Returns the number of variables for the given kind.
        /// </summary>
        /// <param name="kind">The kind.</param>
        /// <returns></returns>
        public int VarCount(Kind kind)
        {
            // start here brian -- see page 239
            switch (kind)
            {
                case Kind.Static:
                    return this.staticCount;
                case Kind.Field:
                    return this.fieldCount;
                case Kind.Arg:
                    return this.argCount;
                case Kind.Var:
                    return this.varCount;
                case Kind.None:
                    return -1;
                default:
                    break;
            }

            return -1;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public Kind KindOf(string name)
        {
            Kind result = Kind.None;

            Identifier identifier = this.GetIdentifierByName(name);

            // check if found
            if (identifier != null)
            {
                result = identifier.Kind;
            }

            return result;
        }

        /// <summary>
        /// Returns the type of the named identifier in the current scope.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns></returns>
        public string TypeOf(string name)
        {
            string result = String.Empty;

            Identifier identifier = this.GetIdentifierByName(name);

            if (identifier != null)
            {
                result = identifier.Type;
            }

            return result;
        }

        /// <summary>
        /// Returns the index assigned to named identifier.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public int IndexOf(string name)
        {
            int result = -1;

            Identifier identifier = this.GetIdentifierByName(name);

            if (identifier != null)
            {
                result = identifier.Index;
            }

            return result;
        }

        /// <summary>
        /// Looks up the identifier.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns></returns>
        public Identifier GetIdentifierByName(string name)
        {
            // warning this shouldn't be part of the API, probably should be private
            // TODO - warning should we be searching both scopes here?
            Identifier identifier = null;
            
            if (this.subRoutineScope.ContainsKey(name))
            {
                identifier = this.subRoutineScope[name];
            }
            else if(this.classScope.ContainsKey(name))
            {
                identifier = this.classScope[name];
            }

            return identifier;
        }

        /// <summary>
        /// Sets the index of the identifier.
        /// </summary>
        /// <param name="identifier">The identifier.</param>
        /// <returns></returns>
        private void IncrementVarKindCount(Identifier identifier)
        {
            switch (identifier.Kind)
            {
                case Kind.Static:
                    this.staticCount++;
                    identifier.Index = this.staticCount;
                    break;
                case Kind.Field:
                    this.fieldCount++;
                    identifier.Index = this.fieldCount;
                    break;
                case Kind.Arg:
                    this.argCount++;
                    identifier.Index = this.argCount;
                    break;
                case Kind.Var:
                    this.varCount++;
                    identifier.Index = this.varCount;
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// Resets the local variable indexes.
        /// </summary>
        private void ResetSubRoutineVarCountsIndex()
        {
            this.argCount = 0;
            this.varCount = 0;
            this.subRoutineScopeIdentifierIndex = 0;
            
        }
    }
}
