using System.Threading.Tasks;
using Ordering.Domain.AggregateModels.OrderAggregate;
using Ordering.Domain.Contracts;

namespace Ordering.API.Commands
{
    // Following a simplfied CQRS pattern, this is a command object that creates a new order
    public class CreateOrderCommandHandler
    {
        private readonly IOrderRepository repository;

        public CreateOrderCommandHandler(IOrderRepository orderRepository)
        {
            repository = orderRepository;
        }

        public async Task<string> Handle(CreateOrderCommand createOrderCommand)
        {
            // Create Order domain aggregate
            var order = new Order(
                null,
                createOrderCommand.CustomerId,
                createOrderCommand.BasketId,
                createOrderCommand.OrderDate,
                createOrderCommand.BuyerId,
                createOrderCommand.Total,
                createOrderCommand.OrderDetails);

            // Add Order to DataStore
            string orderId = await repository.Add(order);
            //TODO - Go to Order and Get Order ID, Change type to string
            return orderId;
        }
    }
}