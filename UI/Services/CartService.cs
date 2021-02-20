using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using MusicStore.Helper;
using MusicStore.Models;

namespace MusicStore.Services
{
    public class CartService : ICartService
    {
        private const string cookieName = ".musicstore";
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IRestClient _restClient;
        private readonly string baseUrl = "api/BasketServices";

        public CartService(IRestClient restClient, IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
            _restClient = restClient;
        }

        public async Task<BasketDto> GetCart()
        {
            var response =  await _restClient.GetAsync<List<BasketDto>>($"{baseUrl}/Basket/{GetBasketId()}");
            return response.Data?.FirstOrDefault();
        }

        public async Task<BasketDto> AddItem(int productId)
        {
            var response = await _restClient.PostAsync<BasketDto>($"{baseUrl}/Basket/{GetBasketId()}/item/{productId}");
            return response.Data;
        }

        public async Task<bool> RemoveItem(int productId)
        {
            var response =  await _restClient.DeleteAsync($"{baseUrl}/Basket/{GetBasketId()}/item/{productId}");
            return response.Data;
        }

        public async Task<bool> Checkout(OrderCreateDto orderCreateDto)
        {
            var response = await _restClient.PostAsync<dynamic>($"{baseUrl}/checkout",orderCreateDto);
            if (response.HttpResponseMessage.IsSuccessStatusCode)
            {
                await _restClient.DeleteAsync($"{baseUrl}/{orderCreateDto.BasketId}");
                SetBasketId();
            }
            
            return response.HttpResponseMessage.IsSuccessStatusCode;
        }

        /// <summary>
        ///     Get or Create Cart ID using a persistent cookie
        /// </summary>
        /// <returns></returns>
        public string GetBasketId()
        {
            string cartId = _httpContextAccessor.HttpContext.Request.Cookies[cookieName] ?? SetBasketId();

            return cartId;
        }

        /// <summary>
        /// Sets the cookie value for the basketId or creates a new ID if null
        /// </summary>
        /// <param name="basketId">The <see cref="BasketDto.BasketId">Basket ID</see>.
        /// If null create a new ID</param>
        /// <returns>the new The <see cref="BasketDto.BasketId">Basket ID</see></returns>
        private string SetBasketId(string basketId = null)
        {
            //A GUID to hold the basketId. 
            if (string.IsNullOrWhiteSpace(basketId))
                basketId = Guid.NewGuid().ToString();

            var options = new CookieOptions
            {
                Expires = DateTime.Now.AddDays(7),
                HttpOnly = true
            };

            _httpContextAccessor.HttpContext.Response.Cookies.Append(cookieName, basketId, options);
            return basketId;
        }
    }
}