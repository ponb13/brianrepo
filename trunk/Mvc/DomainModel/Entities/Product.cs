using System;
using System.Collections.Generic;
using System.Data.Linq.Mapping;
using System.Linq;
using System.Text;

namespace DomainModel.Entities
{
    [Table(Name="Products")]
    public class Product
    {
        [Column (IsPrimaryKey = true,IsDbGenerated = true, AutoSync = AutoSync.OnInsert)]
        public int ProductId { get; set; }
        [Column]
        public string Name { get; set; }
        [Column]
        public string Description { get; set; }
        [Column]
        public decimal Price { get; set; }
        [Column]
        public string Category { get; set; }
    }
}
