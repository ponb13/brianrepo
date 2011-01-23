using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Visitor
{
    class Recipe: ICostable
    {
        public Recipe()
        {
            CostableItems = new List<ICostable>();
        }

        public decimal Cost {get;set;}
        public List<ICostable> CostableItems
        {
            get;
            set;
        }

        public void Accept(IVisitor visitor)
        {
            visitor.Visit(this);
        }
    }
}
