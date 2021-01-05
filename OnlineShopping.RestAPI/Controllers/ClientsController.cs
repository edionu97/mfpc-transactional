using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using OnlineShopping.Models;
using OnlineShopping.Services;

namespace OnlineShopping.RestAPI.Controllers
{
    [ApiController]
    [Route("api/clients")]
    public class ClientsController : ControllerBase
    {
        private readonly IShoppingService _shoppingService;

        public ClientsController(IShoppingService shoppingService)
        {
            _shoppingService = shoppingService;
        }

        /// <summary>
        /// This represents the endpoint for registering a new client
        /// </summary>
        /// <param name="message">the information extracted from the request body</param>
        /// <returns></returns>
        [HttpPost("register")]
        public async Task<IActionResult> RegisterClientAsync([FromBody] Client message)
        {
            try
            {
                //register the client
                await _shoppingService
                    .RegisterNewClientAsync(
                        message.FirstName,
                        message.LastName,
                        message.CNP,
                        message.PhoneNumber,
                        message.Email);

                //if everything  is ok then return the ok response
                return Ok(new
                {
                    Message = "The user registered successfully"
                });
            }
            catch (Exception exception)
            {
                return Problem(exception.Message);
            }
        }

    }
}
