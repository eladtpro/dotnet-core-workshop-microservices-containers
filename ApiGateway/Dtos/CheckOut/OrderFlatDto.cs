using System;

namespace ApiGateway.API.Dtos.CheckOut
{
    public class OrderFlatDto
    {
        public string OrderId { get; set; }
        public string ShoppingBasketId { get; set; }
        public DateTime OrderDate { get; set; }
        public string Username { get; set; }
        public decimal Total { get; set; }
    }
}