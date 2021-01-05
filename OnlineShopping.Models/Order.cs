using System;
using System.Collections.Generic;
using System.Data;
using DatabaseSystem.Utility.Attributes;
using Newtonsoft.Json;

namespace OnlineShopping.Models
{
    public class Order
    {
        [Map(nameof(OrderId), SqlDbType.Int, true)]
        [JsonProperty("orderId")]
        public int OrderId { get; set; }

        [Map(nameof(ClientId), SqlDbType.Int)]
        [JsonProperty("clientId")]
        public int ClientId { get; set; }

        [Map(nameof(ProductId), SqlDbType.Int)]
        [JsonProperty("productId")]
        public int ProductId { get; set; }

        [Map(nameof(OrderDate), SqlDbType.DateTime)]
        [JsonProperty("orderDate")]
        public DateTime OrderDate { get; set; }

        [Map(nameof(ItemsNo), SqlDbType.Int)]
        [JsonProperty("itemsCount")]
        public int ItemsNo { get; set; }

        [JsonProperty("orderedProducts")]
        public IList<Product> OrderedProducts { get; set; } = new List<Product>();
    }
}
