using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using OnlineShopping.Models;
using OnlineShopping.Services;

namespace OnlineShopping.RestAPI.Controllers
{
    [Route("api/products")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly IShoppingService _shoppingService;

        public ProductsController(IShoppingService shoppingService)
        {
            _shoppingService = shoppingService;
        }

        /// <summary>
        /// This method represents the endpoint for adding a product
        /// </summary>
        /// <param name="message">the product that will be added</param>
        /// <returns>either ok either problem action result</returns>
        [HttpPost("add-new")]
        public async Task<IActionResult> AddProductAsync([FromBody] Product message)
        {
            try
            {
                //add the new product
                await _shoppingService.AddProductAsync(
                    message.ProductName,
                    message.Price,
                    message.Model,
                    message.ProductCode);

                //return success
                return Ok(new
                {
                    Message = "Product added successfully"
                });
            }
            catch (Exception e)
            {
                return Problem(e.Message);
            }
        }

        /// <summary>
        /// This method it is used for getting all the products from database
        /// </summary>
        /// <returns>a list of action result that contains either the result list, either the problem</returns>
        [HttpGet("all")]
        public async Task<IActionResult> GetAllProductsAsync()
        {
            try
            {
                //get all products
                var allProducts 
                    = await _shoppingService.GetAllProductsAsync();

                //return the success response
                return Ok(new
                {
                    Products = allProducts
                });
            }
            catch (Exception e)
            {
                return Problem(e.Message);
            }
        }
    }
}
