using System.Data;
using DatabaseSystem.Utility.Attributes;

namespace OnlineShopping.Models
{
    public class Client
    {
        [Map(nameof(ClientId), SqlDbType.Int, true)]
        public int ClientId { get; set; }

        [Map(nameof(FirstName), SqlDbType.NVarChar)]
        public string FirstName { get; set; }

        [Map(nameof(LastName), SqlDbType.NVarChar)]
        public string LastName { get; set; }

        [Map(nameof(CNP), SqlDbType.NVarChar)]
        // ReSharper disable once InconsistentNaming
        public string CNP { get; set; }

        [Map(nameof(PhoneNumber), SqlDbType.NVarChar)]
        public string PhoneNumber { get; set; }

        [Map(nameof(Email), SqlDbType.NVarChar)]
        public string Email { get; set; }
    }
}
