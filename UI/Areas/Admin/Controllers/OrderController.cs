using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MusicStore.Services;

namespace MusicStore.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class OrderController : Controller
    {
        private readonly IOrderService _orderService;

        public OrderController(IOrderService orderService)
        {
            _orderService = orderService;
        }

        public async Task<IActionResult> Index()
        {
            var model = await _orderService.GetOrders();
            return View(model);
        }

        public async Task<IActionResult> Details(string orderId)
        {
            var model = await _orderService.GetOrder(orderId);
            return View(model);

        }

    }
}