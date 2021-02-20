using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MusicStore.Helper;
using MusicStore.Models;
using MusicStore.Services;
using MusicStore.ViewModels;

namespace MusicStore.Controllers
{
    public class ShoppingCartController : Controller
    {
        private readonly ICartService _cartService;
        private readonly ILogger<ShoppingCartController> _logger;


        public ShoppingCartController(ICartService cartService, ILogger<ShoppingCartController> logger)
        {
            _cartService = cartService;
            _logger = logger;
        }


        //
        // GET: /ShoppingCart/
        public async Task<IActionResult> Index()
        {
            BasketDto basket;
            basket = await _cartService.GetCart() ?? new BasketDto();
            // Set up our ViewModel
            var viewModel = new ShoppingCartViewModel
            {
                CartItems = basket.CartItems,
                CartTotal = basket.CartTotal
            };

            // Return the view
            return View(viewModel);
        }

        //
        // GET: /ShoppingCart/AddToCart/5

        public async Task<IActionResult> AddToCart(int id, CancellationToken requestAborted)
        {
            // Add it to the shopping cart
            await _cartService.AddItem(id);

            _logger.LogInformation($"Album {id} was added to the cart.");

            // Go back to the main store page for more shopping
            return RedirectToAction("Index");
        }

        //
        // AJAX: /ShoppingCart/RemoveFromCart/5
        public async Task<IActionResult> RemoveFromCart(int id, CancellationToken requestAborted)
        {
            // Remove from cart
            await _cartService.RemoveItem(id);

            TempData[ToastrMessage.Success] = "Successfully Removed Album";
            
            return RedirectToAction("Index");
        }
    }
}