using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DomainModel.Abstract;
using DomainModel.Entities;

namespace DomainModel.Concrete
{
    public class FakeOrderRepository : IOrdersRepository
    {
        private static IQueryable<Order> _orders = new List<Order>{ 
                                                                new Order() { Name = "Some order", OrderId = 1 },
                                                                new Order() { Name = "another order", OrderId = 2 }
                                                                }.AsQueryable();

        public IQueryable<Order> Orders
        {
            get { return _orders; }
        }
    }
}
