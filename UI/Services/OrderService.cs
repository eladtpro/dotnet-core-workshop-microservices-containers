using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MusicStore.Helper;
using MusicStore.Models;

namespace MusicStore.Services
{
    public class OrderService : IOrderService
    {
        private readonly IRestClient _restClient;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly string baseUrl = "api/OrderingServices";

        public OrderService(IRestClient restClient, IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
            _restClient = restClient;
        }

        public async Task<List<OrderDto>> GetOrders()
        {
            var response =  await _restClient.GetAsync<List<OrderDto>>($"{baseUrl}/Orders");
            return response.Data;

        }


        public async Task<OrderIndexDto> GetOrder(string orderId)
        {
            var response = await _restClient.GetAsync<OrderIndexDto>($"{baseUrl}/v1/Order/{orderId}");
            return response.Data;

        }


    }
}
