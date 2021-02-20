using System.Threading.Tasks;
using Ordering.Domain.Contracts;

namespace Ordering.API.Queries
{
    public class OrderQueries : IOrderQueries
    {
        private readonly IOrderRepository _orderRepository;

        public OrderQueries(IOrderRepository orderRepository)
        {
            _orderRepository = orderRepository;
        }

        // Query object allows a simple query against the datastore without having to 
        // adhere to the constraints of the domain Orderaggregate object. The query 
        // bypasses the Order domain object. Note how it uses a dynamic type.
        public async Task<dynamic> GetOrderCosmos(string orderId, string corrleationId)
        {
            return await _orderRepository.GetAll(orderId, corrleationId);
        }

        // adhere to the constraints of the domain Orderaggregate object. The query 
        // bypasses the Order domain object. Note how it uses a dynamic type.
        public async Task<dynamic> GetOrders(string corrleationId)
        {
            return await _orderRepository.GetById(corrleationId);
        }
    }
}