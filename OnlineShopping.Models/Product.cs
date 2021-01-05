using System.Data;
using DatabaseSystem.Utility.Attributes;

namespace OnlineShopping.Models
{
    public class Product
    {
        [Map(nameof(ProductId), SqlDbType.Int, true)]
        public int ProductId { get; set; }

        [Map(nameof(ProductName), SqlDbType.NVarChar)]
        public string ProductName { get; set; }

        [Map(nameof(Price), SqlDbType.Int)]
        public int Price { get; set; }

        [Map(nameof(Model), SqlDbType.NVarChar)]
        public string Model { get; set; }

        [Map(nameof(ProductCode), SqlDbType.Int)]
        public int ProductCode { get; set; }
    }
}
