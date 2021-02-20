using System.Collections.Generic;
using System.Threading.Tasks;
using MusicStore.Models;

namespace MusicStore.Services
{
    public interface IOrderService
    {
        Task<List<OrderDto>> GetOrders();
        Task<OrderIndexDto> GetOrder(string orderId);
    }
}