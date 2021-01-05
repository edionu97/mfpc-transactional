using System.Data;
using DatabaseSystem.Utility.Attributes;
using Newtonsoft.Json;

namespace OnlineShopping.Models
{
    public class Product
    {
        [Map(nameof(ProductId), SqlDbType.Int, true)]
        [JsonProperty("productId")]
        public int ProductId { get; set; }

        [Map(nameof(ProductName), SqlDbType.NVarChar)]
        [JsonProperty("productName")]
        public string ProductName { get; set; }

        [Map(nameof(Price), SqlDbType.Int)]
        [JsonProperty("price")]
        public int Price { get; set; }

        [Map(nameof(Model), SqlDbType.NVarChar)]
        [JsonProperty("model")]
        public string Model { get; set; }

        [Map(nameof(ProductCode), SqlDbType.Int)]
        [JsonProperty("serialCode")]
        public int ProductCode { get; set; }
    }
}
