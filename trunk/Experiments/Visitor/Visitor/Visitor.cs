using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Visitor
{
    class Visitor : IVisitor
    {
        public void Visit(Product product)
        {
            product.Cost = 5;
        }

        public void Visit(Recipe recipe)
        {
            foreach (ICostable costableItem in recipe.CostableItems)
            {
                costableItem.Accept(this);
                recipe.Cost += costableItem.Cost;
            }
        }

        public void Visit(Choice Choice)
        {
            foreach (ICostable costableItem in Choice.CostableItems)
            {
                costableItem.Accept(this);
                Choice.Cost += costableItem.Cost;
            }
        }
    }
}
