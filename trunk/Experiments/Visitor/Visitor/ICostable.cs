using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Visitor
{
    interface ICostable
    {
        decimal Cost { get; set;}

        void Accept(IVisitor visitor);
    }
}
