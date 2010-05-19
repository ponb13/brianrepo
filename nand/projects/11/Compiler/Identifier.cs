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
    }

}
