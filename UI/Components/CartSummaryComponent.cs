using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MusicStore.Models;
using MusicStore.Services;

namespace MusicStore.Components
{
    [ViewComponent(Name = "CartSummary")]
    public class CartSummaryComponent : ViewComponent
    {
        private readonly ICartService _cartService;

        public CartSummaryComponent(ICartService cartService)
        {
            _cartService = cartService;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            BasketDto basket;
            basket = await _cartService.GetCart() ?? new BasketDto();

            ViewBag.CartCount = basket.ItemCount;
            ViewBag.CartSummary = string.Join("\n", basket.CartItems.Select(c => c.Name).Distinct());

            return View();
        }
    }
}