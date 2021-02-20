using System;

namespace Ordering.API.Application.Dtos
{
    public class FlattenedOrderDto
    {
        public string OrderId { get; set; }
        public string ShoppingBasketId { get; set; }
        public DateTime OrderDate { get; set; }
        public string Username { get; set; }
        public decimal Total { get; set; }
    }
}