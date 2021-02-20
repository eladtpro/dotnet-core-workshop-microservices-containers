using System.Threading.Tasks;
using MusicStore.Models;

namespace MusicStore.Services
{
    public interface ICartService
    {
        Task<BasketDto> GetCart();
        Task<BasketDto> AddItem(int productId);
        string GetBasketId();
        Task<bool> RemoveItem(int productId);
        Task<bool> Checkout(OrderCreateDto orderCreateDto);
    }
}