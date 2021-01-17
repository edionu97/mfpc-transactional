using Newtonsoft.Json;

namespace OnlineShopping.RestAPI.Messages
{
    public class OrderedProductMessage
    {
        [JsonProperty("productId")]
        public int ProductId { get; set; }

        [JsonProperty("itemsNo")]

        public int ItemsNo { get; set; }

        public void Deconstruct(out int productId, out int itemsNo)
        {
            productId = ProductId;
            itemsNo = ItemsNo;
        }
    }
}
