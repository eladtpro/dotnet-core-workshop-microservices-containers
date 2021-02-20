using System.Collections.Generic;
using System.Threading.Tasks;
using ApiGateway.API.Dtos.CheckOut;
using ApiGateway.API.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using RestCommunication;
using ServiceDiscovery;

namespace ApiGateway.API.Controllers
{
    /// <summary>
    ///     Gateway microservice that manages Ordering experience
    /// </summary>
    [ApiVersion("1.0")]
    [ApiVersion("2.0")]
    [ApiController]
    [Route("api/[controller]")]
    public class OrderingServicesController : ControllerBase
    {
        private readonly IRestClient _restClient;

        public OrderingServicesController(IRestClient restClient)
        {
            _restClient = restClient;
        }

        /// <summary>
        ///     Get Details for Specified Order - Version 2.0
        /// </summary>
        /// <param name="orderId">Identifer for an order</param>
        /// <returns>Details for specified Order</returns>
        [ProducesResponseType(typeof(OrderDto), 200)]
        [HttpGet("v{version:apiVersion}/Order/{orderId}", Name = "GetOrdersOrderingRoutev2")]
        public async Task<IActionResult> GetOrderv2(string orderId, string version)
        {
            // Generate correlationToken
            var correlationToken = CorrelationTokenManager.GenerateToken();

            var result = await _restClient.GetAsync<dynamic>(ServiceEnum.Ordering,
                $"api/ordering/v{version}/Order/{orderId}/{correlationToken}");

            if (result == null)
                return BadRequest($"Order with Id {orderId} does not exist");

            return new ObjectResult(result);
        }

        /// <summary>
        ///     Gets All Orders
        /// </summary>
        /// <param name="orderId">Identifer for an order</param>
        /// <returns>Details for specified Order</returns>
        [ProducesResponseType(typeof(List<OrderFlatDto>), 200)]
        [HttpGet("Orders", Name = "GetAllOrdersOrderingRoute")]
        public async Task<IActionResult> GetAllOrders()
        {
            // Generate correlationToken
            var correlationToken = CorrelationTokenManager.GenerateToken();
            var result = await _restClient.GetAsync<List<OrderFlatDto>>(ServiceEnum.Ordering,
                $"api/Ordering/v1.0/Orders/{correlationToken}");

            if (result.Count < 1)
                return BadRequest("No orders exist");

            return new ObjectResult(result);
        }

        /// <summary>
        ///     Test Exception Handling - Simulate 500 Error
        /// </summary>
        /// <returns>Returns Http Status Code 500 - Internal Server Error</returns>
        [ProducesResponseType(500)]
        [HttpGet("SimulateError", Name = "GatewaySimulateErrorRoute")]
        public async Task<IActionResult> SimulateError()
        {
            // Generate correlationToken
            var correlationToken = CorrelationTokenManager.GenerateToken();
            var result = await _restClient.GetAsync<List<OrderFlatDto>>(ServiceEnum.Ordering,
                $"api/Ordering/Orders/SimulateError/{correlationToken}");

            return new ObjectResult(result);
        }
    }
}