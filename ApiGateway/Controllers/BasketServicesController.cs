using System.Collections.Generic;
using System.Threading.Tasks;
using ApiGateway.API.Dtos.Basket;
using ApiGateway.API.Dtos.Catalog;
using ApiGateway.API.Dtos.CheckOut;
using ApiGateway.API.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using RestCommunication;
using ServiceDiscovery;
using Utilities;

namespace ApiGateway.API.Controllers
{
    /// <summary>
    ///     Gateway microservice that manages Shopping Basket experience
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class BasketServicesController : ControllerBase
    {
        private readonly IRestClient client;

        public BasketServicesController(IRestClient restClient)
        {
            client = restClient;
        }

        /// <summary>
        ///     Gets All Shopping Baskets.
        /// </summary>
        /// <returns>List of line items that make up a shopping basket</returns>
        [ProducesResponseType(typeof(BasketDto), 200)]
        [HttpGet("Baskets", Name = "GetAllBasketsGatewayRoute")]
        public async Task<IActionResult> GetAllBaskets()
        {
            // Generate correlationToken
            var correlationToken = CorrelationTokenManager.GenerateToken();

            // Get music
            var result = await client.GetAsync<List<BasketDto>>(ServiceEnum.Basket,
                $"api/Basket/Baskets/{correlationToken}");

            if (result == null || result.Count < 1)
                return BadRequest("No Shopping Baskets exist");

            return new ObjectResult(result);
        }

        /// <summary>
        ///     Gets Specfied Shopping Basket and its Line Items
        /// </summary>
        /// <param name="basketId">Identifier for user shopping basket</param>
        /// <returns>List of line items that make up a shopping basket</returns>
        [ProducesResponseType(typeof(BasketItemDto), 200)]
        [HttpGet("Basket/{basketId}", Name = "GetBasketGatewayRoute")]
        public async Task<IActionResult> GetBasket(string basketId)
        {
            // Generate correlationToken
            var correlationToken = CorrelationTokenManager.GenerateToken();

            // Get music
            var response = await client.GetAsync<List<BasketDto>>(ServiceEnum.Basket,
                $"api/Basket/Basket/{basketId}/{correlationToken}");

            if (response == null || response.Count < 1)
                return BadRequest($"Shopping Basket for Id {basketId} does not exist");

            return new ObjectResult(response);
        }

        /// <summary>
        ///     Adds New Line Item to Specified Shopping Basket
        /// </summary>
        /// <param name="productId">Product Identifier</param>
        /// <param name="basketId">Identifier for user shopping basket</param>
        /// <returns>The newly-created line item</returns>
        [ProducesResponseType(typeof(BasketItemDto), 201)]
        [HttpPost("Basket/{basketId}/item/{productId:int}", Name = "NewBasketLineItemGatewayRoute")]
        public async Task<IActionResult> AddItemToBasket(string basketId, int productId)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var correlationToken = CorrelationTokenManager.GenerateToken();

            Product product = null;

            // Send product information and basketId to the Basket microservice
            // Note: Sending querystring parameters in the post.
            var response = await client.PostAsync<dynamic>(ServiceEnum.Basket,
                $"api/Basket/?productId={productId}&correlationToken={correlationToken}&basketId={basketId}", product);

            return CreatedAtRoute("NewBasketLineItemGatewayRoute", response);
        }

        /// <summary>
        ///     Converts Shopping Basket to an Order
        /// </summary>
        /// <param name="NewOrderDto">Object that represents New Order</param>
        /// <returns>The newly-created line item</returns>
        [ProducesResponseType(typeof(OrderDto), 201)]
        [HttpPost("CheckOut", Name = "CheckOutGatewayRoute")]
        public async Task<IActionResult> CheckOut([FromBody] NewOrderDto newOrderDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var correlationToken = CorrelationTokenManager.GenerateToken();

            // Note: Sending querystring parameters in the post.
            var response = await client.PostAsync<NewOrderDto>(ServiceEnum.Basket,
                $"api/Basket/CheckOut/?correlationToken={correlationToken}", newOrderDto);

            if (response == null)
                return BadRequest($"Shopping Basket with Id {newOrderDto.BasketId} could not be found");

            return Accepted("Order Being Created");
        }

        /// <summary>
        ///     Removes Specified Line Item from a Shopping Basket
        /// </summary>
        /// <param name="basketId">Identifier for user shopping basket</param>
        /// <param name="productId">Product Identifier</param>
        /// <returns>Summary of shopping basket state</returns>
        [ProducesResponseType(typeof(BasketItemRemoveDto), 200)]
        [HttpDelete("Basket/{basketId}/item/{productId:int}")]
        public async Task<IActionResult> DeleteLineItem(string basketId, int productId)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var correlationToken = CorrelationTokenManager.GenerateToken();

            // Note: Sending querystring parameters in the post.
            var success = await client.DeleteAsync(ServiceEnum.Basket,
                $"api/basket/{basketId}/lineitem/{productId}?correlationToken={correlationToken}");

            if (success)
                return NoContent();

            return NotFound();
        }

        /// <summary>
        ///     Removes entire Shopping Basket and all its Line Items
        /// </summary>
        /// <param name="basketId">Identifier for user shopping basket</param>
        /// <returns></returns>
        [ProducesResponseType(204)]
        [HttpDelete("Basket/{basketId}")]
        public async Task<IActionResult> Delete(string basketId)
        {
            Guard.ForNullOrEmpty("basketId", "Must include BasketID value");

            var correlationToken = CorrelationTokenManager.GenerateToken();
            // Note: Sending querystring parameters in the post.
            var response = await client.DeleteAsync(ServiceEnum.Basket,
                $"api/Basket/?basketId={basketId}&correlationToken={correlationToken}");

            return NoContent();
        }
    }
}