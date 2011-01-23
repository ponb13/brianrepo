using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Visitor
{
    class Program
    {
        static void Main(string[] args)
        {
            Recipe recipe = new Recipe();
            Choice chipsOrMash = new Choice();

            Product beans = new Product();
            Product sausages = new Product();
            Product chips = new Product();
            Product mash = new Product();

            recipe.CostableItems.Add(beans);
            recipe.CostableItems.Add(sausages);

            recipe.CostableItems.Add(chipsOrMash);
            chipsOrMash.CostableItems.Add(chips);
            chipsOrMash.CostableItems.Add(mash);

            Visitor visitor = new Visitor();
            recipe.Accept(visitor);

            Console.WriteLine(recipe.Cost);
        }
    }
}
