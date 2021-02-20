using System.Threading.Tasks;
using Ordering.Domain.AggregateModels.BuyerAggregate;
using Ordering.Domain.AggregateModels.OrderAggregate;

namespace Ordering.Domain.Contracts
{
    public interface IOrderRepository
    {
        Task<string> Add(Order entity);
        Task<dynamic> GetAll(string orderId, string correlationToken);
        Task<dynamic> GetById(string correlationToken);

    }
}