using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Ordering.API.Application.Dtos;
using Ordering.API.Queries;
using Utilities;

namespace Ordering.API.Controllers
{
    [ApiVersion("1.0")]
    [ApiVersion("2.0")]
    [ApiController]
    [Route("api/[controller]")]
    public class OrderingController : ControllerBase
    {
        private readonly IOrderQueries _orderQueries;

        public OrderingController(IOrderQueries orderQueries)
        {
            _orderQueries = orderQueries;
        }

        /// <summary>
        ///     Get Detail for Specified Order - Ver 1.0
        /// </summary>
        /// <param name="orderId">Identifer for an order</param>
        /// <returns>Details for specified Order</returns>
        [ProducesResponseType(typeof(OrderDto), 200)]
        [HttpGet("v{version:apiVersion}/Order/{orderId}/{correlationToken}", Name = "GetOrdersRoute")]
        public async Task<IActionResult> GetOrder(string orderId, string correlationToken)
        {
            Guard.ForNullOrEmpty(orderId, "OrderId");
            Guard.ForNullOrEmpty(correlationToken, "correlationToken");

            var orderresult = await _orderQueries.GetOrderCosmos(orderId, correlationToken);

            if (orderresult == null)
                return new ObjectResult(orderresult);

            // Map Order to OrderDto
            var OrderDto = Mapper.MapToOrderDto(orderresult);
            return new ObjectResult(OrderDto);
        }

        /// <summary>
        ///     Get Detail for Specified Order - Ver 2.0
        /// </summary>
        /// <param name="orderId">Identifer for an order</param>
        /// <returns>Details for specified Order</returns>
        [ProducesResponseType(typeof(OrderDto), 200)]
        [HttpGet("v{version:apiVersion}/Order/{orderId}/{correlationToken}"), MapToApiVersionAttribute("2.0")]
        public async Task<IActionResult> GetOrderv2(string orderId, string correlationToken)
        {
            Guard.ForNullOrEmpty(orderId, "OrderId");
            Guard.ForNullOrEmpty(correlationToken, "correlationToken");

            var orderresult = await _orderQueries.GetOrderCosmos(orderId, correlationToken);

            if (orderresult == null)
                return new ObjectResult(orderresult);

            // Map Order to OrderDto
            var OrderDto = Mapper.MapToOrderDto(orderresult);
            return new ObjectResult(OrderDto);
        }



        /// <summary>
        ///     Gets All Orders
        /// </summary>
        /// <param name="orderId">Identifer for an order</param>
        /// <returns>Details for specified Order</returns>
        [ProducesResponseType(typeof(OrderDto), 200)]
        [HttpGet("v{version:apiVersion}/Orders/{correlationToken}", Name = "GetAllOrdersRoute")]
        public async Task<IActionResult> GetAllOrders(string correlationToken)
        {
            Guard.ForNullOrEmpty(correlationToken, "correlationToken");

            var orders = await _orderQueries.GetOrders(correlationToken);

            if (orders == null)
                return new ObjectResult(new List<OrderDetailDto>());

            // Map Order to OrderDto
            var OrderDto = Mapper.MapToOrdersDto(orders);
            return new ObjectResult(OrderDto);
        }

        /// <summary>
        ///     Simulate 500 Error
        /// </summary>
        /// <param name="orderId">Identifer for an order</param>
        /// <param name="correlationToken">Tracks request - Can be any value</param>
        /// <returns>Details for specified Order</returns>
        [ProducesResponseType(typeof(OrderDto), 200)]
        [HttpGet("Orders/SimulateError/{correlationToken}", Name = "SimulateErrorRoute")]
        public async Task<IActionResult> SimulateError(string correlationToken)
        {
            return await Task.FromResult(StatusCode(StatusCodes.Status500InternalServerError));
        }
    }
}