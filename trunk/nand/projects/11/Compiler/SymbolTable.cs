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
        private int classStaticCount = -1;
        private int classFieldCount = -1;
        private int localArgCount = -1;
        private int localVarCount = -1;

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
            this.ResetLocalVariableIndexes();
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
            Identifier identifier = new Identifier
            {
                Name = name,
                Type = type,
                Kind = kind
            };

            this.SetIdentifierIndex(identifier);

            if (kind == Kind.Static || kind == Kind.Field)
            {
                this.classScope.Add(name, identifier);
            }
            else if (kind == Kind.Var || kind == Kind.Arg)
            {
                this.subRoutineScope.Add(name, identifier);
            }
        }

        /// <summary>
        /// Returns the number of variables for the given kind.
        /// </summary>
        /// <param name="kind">The kind.</param>
        /// <returns></returns>
        public int VarCount(Kind kind)
        {
            switch (kind)
            {
                case Kind.Static:
                    return this.classStaticCount;
                case Kind.Field:
                    return this.classFieldCount;
                case Kind.Arg:
                    return this.localArgCount;
                case Kind.Var:
                    return this.localVarCount;
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
        private Identifier GetIdentifierByName(string name)
        {
            // TODO - warning should we be searching both scopes here?
            Identifier identifier = this.subRoutineScope[name];

            if (identifier == null)
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
        private void SetIdentifierIndex(Identifier identifier)
        {
            switch (identifier.Kind)
            {
                case Kind.Static:
                    this.classStaticCount++;
                    identifier.Index = this.classStaticCount;
                    break;
                case Kind.Field:
                    this.classFieldCount++;
                    identifier.Index = this.classFieldCount;
                    break;
                case Kind.Arg:
                    this.localArgCount++;
                    identifier.Index = this.localArgCount;
                    break;
                case Kind.Var:
                    this.localVarCount++;
                    identifier.Index = this.localVarCount;
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// Resets the local variable indexes.
        /// </summary>
        private void ResetLocalVariableIndexes()
        {
            this.localArgCount = -1;
            this.localVarCount = -1;
        }
    }
}
