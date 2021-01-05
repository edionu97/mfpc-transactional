using System.Data;
using DatabaseSystem.Utility.Attributes;
using Newtonsoft.Json;

namespace OnlineShopping.Models
{
    public class Client
    {
        [Map(nameof(ClientId), SqlDbType.Int, true)]
        [JsonProperty("clientId")]
        public int ClientId { get; set; }

        [Map(nameof(FirstName), SqlDbType.NVarChar)]
        [JsonProperty("firstName")]
        public string FirstName { get; set; }

        [Map(nameof(LastName), SqlDbType.NVarChar)]
        [JsonProperty("lastName")]
        public string LastName { get; set; }

        [Map(nameof(CNP), SqlDbType.NVarChar)]
        [JsonProperty("cnp")]
        // ReSharper disable once InconsistentNaming
        public string CNP { get; set; }

        [Map(nameof(PhoneNumber), SqlDbType.NVarChar)]
        [JsonProperty("phoneNumber")]
        public string PhoneNumber { get; set; }

        [Map(nameof(Email), SqlDbType.NVarChar)]
        [JsonProperty("email")]
        public string Email { get; set; }
    }
}
