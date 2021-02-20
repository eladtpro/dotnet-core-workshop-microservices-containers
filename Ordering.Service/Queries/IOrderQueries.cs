using System.Threading.Tasks;

namespace Ordering.API.Queries
{
    public interface IOrderQueries
    {
        Task<dynamic> GetOrderCosmos(string orderId, string corrleationId);

        Task<dynamic> GetOrders(string corrleationId);
    }
}