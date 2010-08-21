using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Compiler
{
    public class Identifier
    {
        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>The name.</value>
        public string Name
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the type.
        /// </summary>
        /// <value>The type.</value>
        public string Type
        {
            get;
            set;
        }


        /// <summary>
        /// Gets or sets the kind.
        /// </summary>
        /// <value>The kind.</value>
        public Kind Kind
        {
            get;
            set;
        }

        public Scope IdentifierScope
        {
            get
            {
                if(this.Kind == Kind.Field || this.Kind == Kind.Static)
                {
                    return Scope.ClassLevel;
                }
                else
                {
                    return Scope.MethodLevel;
                }
            }
        }


        /// <summary>
        /// Gets or sets the index.
        /// </summary>
        /// <value>The index.</value>
        public int Index
        {
            get;
            set;
        }

        /// <summary>
        /// currently in use or being defined
        /// </summary>
        public IdentifierUsage Usage
        {
            get;
            set;
        }

        public Segment Segment
        {
            get
            {
                Segment segment = Segment.None; // just default to arguement

                if (this.Kind == Kind.Arg)
                {
                    segment = Segment.Arguement;
                }
                else if (this.Kind == Kind.Var)
                {
                    segment = Segment.Local;
                }
                else if (this.Kind == Kind.Field)
                {
                    // not sure about this!
                    throw new NotImplementedException("you havent figured ouy how to handle fields yet!");
                    segment = Segment.This;
                }
                else if(this.Kind == Kind.Static)
                {
                    throw new NotImplementedException("you havent figured ouy how to handle statics yet!");
                    segment = Segment.Static;
                }

                if (segment == Segment.None)
                {
                    throw new Exception("Something wrong with the segement code in idenetifier, the segment is not getting set in any of the if conditions");
                }

                return segment;
            }
        }
    }

}
