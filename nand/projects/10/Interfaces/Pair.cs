using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Interfaces
{
    /// <summary>
    /// not nice, this isn't an interface, 
    /// hack a roo it can live here for now.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <typeparam name="U"></typeparam>
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
