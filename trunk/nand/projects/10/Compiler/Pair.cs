using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Compiler
{
    public class Pair<T,U>
    {
        /// <summary>
        /// Gets or sets the value1.
        /// </summary>
        /// <value>The value1.</value>
        public T Value1
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the value2.
        /// </summary>
        /// <value>The value2.</value>
        public U Value2
        {
            get;
            set;
        }
    }
}
