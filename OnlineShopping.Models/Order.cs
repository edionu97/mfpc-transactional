using System;
using System.Collections.Generic;
using System.Data;
using DatabaseSystem.Utility.Attributes;

namespace OnlineShopping.Models
{
    public class Order
    {
        [Map(nameof(OrderId), SqlDbType.Int, true)]
        public int OrderId { get; set; }

        [Map(nameof(ClientId), SqlDbType.Int)]
        public int ClientId { get; set; }

        [Map(nameof(ProductId), SqlDbType.Int)]
        public int ProductId { get; set; }

        [Map(nameof(OrderDate), SqlDbType.DateTime)]
        public DateTime OrderDate { get; set; }

        [Map(nameof(ItemsNo), SqlDbType.Int)]
        public int ItemsNo { get; set; }

        public IList<Product> OrderedProducts { get; set; } = new List<Product>();
    }
}
