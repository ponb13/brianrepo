using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Visitor
{
    interface IVisitor 
    {
        void Visit(Product item);
        void Visit(Recipe item);
        void Visit(Choice item);
    }
}
