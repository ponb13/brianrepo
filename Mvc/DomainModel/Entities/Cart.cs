using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DomainModel.Entities
{
    public class Cart
    {
        private List<CartLine> _lines = new List<CartLine>();
        private ShippingDetails _shippingDetails = new ShippingDetails();

        public ShippingDetails ShippingDetails
        {
            get { return _shippingDetails; }
        }

        public IList<CartLine> Lines
        {
            get { return _lines.AsReadOnly(); }
        }

        public void AddItem(Product product, int quantity)
        {
            var line = _lines.FirstOrDefault(l => l.Product.ProductId == product.ProductId);

            if (line == null)
            {
                _lines.Add(new CartLine { Product = product, Quantity = quantity });
            }
            else
            {
                line.Quantity += quantity;
            }
        }

        public void RemoveLine(Product product)
        {
            _lines.RemoveAll(p => p.Product.ProductId == product.ProductId);
        }

        public decimal ComputeTotalValue()
        {
            return _lines.Sum(l => l.Product.Price * l.Quantity);
        }

        public void Clear()
        {
            _lines.Clear();
        }

    }

    public class CartLine
    {
        public Product Product { get; set; }
        public int Quantity { get; set; }
    }
}
