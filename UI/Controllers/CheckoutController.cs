using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MusicStore.Helper;
using MusicStore.Models;
using MusicStore.Services;
using RandomNameGeneratorLibrary;

namespace MusicStore.Controllers
{
    public class CheckoutController : Controller
    {
        private const string PromoCode = "FREE";

        private readonly ILogger<CheckoutController> _logger;

        public CheckoutController(ILogger<CheckoutController> logger)
        {
            _logger = logger;
        }

        //
        // GET: /Checkout/
        public IActionResult AddressAndPayment()
        {
            var nameGenerator = new PersonNameGenerator();
            var placeNameGenerator = new PlaceNameGenerator();
            var rndNum = new Random();

            var firstName = nameGenerator.GenerateRandomMaleFirstName();

            var orderCreateDto = new OrderCreateDto
            {
                FirstName = firstName,
                LastName = nameGenerator.GenerateRandomLastName(),
                Address = rndNum.Next(1000, 9999).ToString() + " " + placeNameGenerator.GenerateRandomPlaceName(),
                PostalCode = rndNum.Next(10000, 99999).ToString(),
                City = placeNameGenerator.GenerateRandomPlaceName(),
                State = "OH",
                Phone = "(" + rndNum.Next(100, 999).ToString() + ")" + rndNum.Next(100, 999).ToString() + "-" + rndNum.Next(1000, 9999).ToString(),
                Country = "USA",
                Email = firstName + "@" + "hotmail.com"
            };

            return View(orderCreateDto);
        }

        //
        // POST: /Checkout/AddressAndPayment
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddressAndPayment(
            [FromServices] ICartService cartService,
            [FromForm] OrderCreateDto orderCreateDto,
            CancellationToken requestAborted)
        {
            if (!ModelState.IsValid) return View(orderCreateDto);

            IFormCollection formCollection = await HttpContext.Request.ReadFormAsync();

            try
            {
                //if (string.Equals(formCollection["PromoCode"].FirstOrDefault(), PromoCode, StringComparison.OrdinalIgnoreCase) == false) return View(orderCreateDto);

                orderCreateDto.Username = $"{orderCreateDto.FirstName}{orderCreateDto.LastName}";
                orderCreateDto.BasketId = cartService.GetBasketId();

                await cartService.Checkout(orderCreateDto);
                
                _logger.LogInformation($"User {orderCreateDto.Username} started checkout of {orderCreateDto.OrderId}.");
                TempData[ToastrMessage.Success]="Thank you for your order";

                return RedirectToAction("index", "Home");
            }
            catch 
            {
                ModelState.AddModelError("", "An error occured whil processing order");
                //Invalid - redisplay with errors
                return View(orderCreateDto);
            }
        }
    }
}