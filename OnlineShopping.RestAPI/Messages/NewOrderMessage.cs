using System;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace OnlineShopping.RestAPI.Messages
{
    public class NewOrderMessage
    {
        [JsonProperty("clientId")]
        public int ClientId { get; set; }

        [JsonProperty("orderedProducts")]
        public List<OrderedProductMessage> OrderedProducts { get; set; }

        public void Deconstruct(out int clientId, out IList<OrderedProductMessage> orderedProducts)
        {
            clientId = ClientId;
            orderedProducts = OrderedProducts;
        }
    }
}
