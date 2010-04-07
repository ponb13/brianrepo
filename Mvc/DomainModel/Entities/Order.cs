using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.Linq;
using System.Data.Linq.Mapping;
using System.Text;

namespace DomainModel.Entities
{
    [Table (Name="Orders")]
    public class Order
    {
        public int OrderId { get; set; }
        public string Name { get; set; }
    }
}
