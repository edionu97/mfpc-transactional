using System;
using System.Linq;
using OnlineShopping.Models;
using System.Threading.Tasks;
using OnlineShopping.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Cors;
using OnlineShopping.RestAPI.Messages;

namespace OnlineShopping.RestAPI.Controllers
{
    [Route("api/orders")]
    [ApiController]
    [EnableCors("AllowOrigin")]
    public class OrdersController : ControllerBase
    {
        private readonly IShoppingService _shoppingService;

        public OrdersController(IShoppingService shoppingService)
        {
            _shoppingService = shoppingService;
        }

        /// <summary>
        /// This method it is used for getting all the available products to be ordered
        /// </summary>
        /// <param name="clientCnp">the client cnp</param>
        /// <returns>a list of available products</returns>
        [HttpGet("available-products-for-client")]
        public async Task<IActionResult> GetAvailableProductsForClientAsync([FromQuery] string clientCnp)
        {
            try
            {
                //get all clients
                var desiredClient = await _shoppingService
                    .GetAllClientsAsync()
                    .ContinueWith(clientsTask =>
                    {
                        return clientsTask.Result.FirstOrDefault(c => c.CNP == clientCnp);
                    });

                //check if the client exists into database
                if (desiredClient == null)
                {
                    throw new Exception("In database does not exist such a client");
                }

                //get all the orders that 
                var orderedProducts = await _shoppingService
                    .GetOrdersForClientAsync(desiredClient.CNP)
                    .ContinueWith(clientOrdersTask =>
                    {
                        var clientOrders = clientOrdersTask.Result;
                        return clientOrders.Select(x => x.ProductId);
                    });

                //get all products
                var availableProductsForOrdering = await _shoppingService
                    .GetAllProductsAsync()
                    .ContinueWith(allProductsTask =>
                    {
                        //convert the products into set 
                        var productSet = orderedProducts.ToHashSet();

                        //return the objects that were no longer ordered
                        var products = allProductsTask.Result;
                        return products.Where(p => !productSet.Contains(p.ProductId));
                    });

                //return the message
                return Ok(new
                {
                    Products = availableProductsForOrdering
                });
            }
            catch (Exception e)
            {
                return Problem(e.Message);
            }
        }

        /// <summary>
        /// This method it is used for adding a new order async
        /// </summary>
        /// <param name="message">the add order message</param>
        /// <returns>either success response either problem response</returns>
        [HttpPost("add-new")]
        public async Task<IActionResult> AddOrderAsync([FromBody] NewOrderMessage message)
        {
            try
            {
                //get the client id
                var (clientId, orderedProducts) = message;

                //iterate through each ordered product
                foreach (var (productId, itemsNo) in orderedProducts)
                {
                    //add a new order
                    await _shoppingService
                        .AddOrderAsync(clientId, productId, itemsNo);
                }

                //return the success response
                return Ok(new
                {
                    Messsage = "Order successfully added"
                });
            }
            catch (Exception e)
            {
                return Problem(e.Message);
            }
        }

        /// <summary>
        /// This method it is used for removing a product from database
        /// </summary>
        /// <param name="message">the required information</param>
        /// <returns>either success response either problem response</returns>
        [HttpDelete("remove-product")]
        public async Task<IActionResult> RemoveProductFromOrderAsync([FromBody] Order message)
        {
            try
            {
                //remove the product
                await _shoppingService.RemoveProductFromOrderAsync(
                    message.ClientId, message.ProductId);

                //return the success response
                return Ok(new
                {
                    Messsage = "Product removed successfully"
                });
            }
            catch (Exception e)
            {
                return Problem(e.Message);
            }
        }

        /// <summary>
        /// This will get all the orders for a specific client
        /// </summary>
        /// <param name="clientCnp">the client cnp</param>
        /// <returns>a list of orders</returns>
        [HttpGet("all/client")]
        public async Task<IActionResult> GetOrdersForClient([FromQuery] string clientCnp)
        {
            try
            {
                //return the orders for the client
                return Ok(new
                {
                    Messsage =
                        await _shoppingService
                            .GetOrdersForClientAsync(clientCnp)
                });
            }
            catch (Exception e)
            {
                return Problem(e.Message);
            }
        }
    }
}
