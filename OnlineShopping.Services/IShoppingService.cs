using System.Collections.Generic;
using System.Threading.Tasks;
using OnlineShopping.Models;

namespace OnlineShopping.Services
{
    public interface IShoppingService
    {
        /// <summary>
        /// Add a product into database
        /// </summary>
        /// <param name="productName">the name of the product</param>
        /// <param name="price">the price of the product</param>
        /// <param name="model">the product model</param>
        /// <param name="productCode">the code of the product</param>
        Task AddProductAsync(string productName, 
                             int price, 
                             string model,
                             int productCode);

        /// <summary>
        /// Add a new client
        /// </summary>
        /// <param name="clientFirstName">the first name of the client</param>
        /// <param name="clientLastName">the second name of the client</param>
        /// <param name="clientCnp">the client cnp</param>
        /// <param name="phoneNumber">the phone number</param>
        /// <param name="email">the email address</param>
        /// <returns></returns>
        Task RegisterNewClientAsync(
            string clientFirstName,
            string clientLastName,
            string clientCnp,
            string phoneNumber,
            string email);
            
        /// <summary>
        /// Add a new order
        /// </summary>
        /// <param name="clientId">the id of the client</param>
        /// <param name="productId">the product id</param>
        /// <param name="numberOfItems">the number of items</param>
        /// <returns></returns>
        Task AddOrderAsync(int clientId, int productId, int numberOfItems);

        /// <summary>
        /// Remove the product from order
        /// </summary>
        Task RemoveProductFromOrderAsync(int clientId, int productId);

        /// <summary>
        /// For a specific client get the orders
        /// </summary>
        /// <param name="clientCnp">the cnp of the client</param>
        /// <returns>a list that represents the clients orders</returns>
        Task<IList<Order>> GetOrdersForClientAsync(string clientCnp);

        /// <summary>
        /// Get all products
        /// </summary>
        /// <returns>a list of products</returns>
        Task<IList<Product>> GetAllProductsAsync();

        /// <summary>
        /// Get a list with all the clients from database
        /// </summary>
        /// <returns>a list of clients</returns>
        Task<IList<Client>> GetAllClientsAsync();

        /// <summary>
        /// Returns a list of products that are no longer bought by user
        /// </summary>
        /// <param name="clientCnp">the client's cnp</param>
        /// <returns>a list of products</returns>
        Task<IList<Product>> GetAllProductsThatCanBeAddedInOrderByUser(string clientCnp);
    }
}
