using System;
using System.Collections.Generic;

namespace ApiGateway.API.Dtos.CheckOut
{
    public class OrderDto
    {
        public OrderDto()
        {
            OrderDetails = new HashSet<OrderDetailDto>();
        }

        public int OrderId { get; set; }
        public DateTime OrderDate { get; set; }
        public string ShoppingBasketId { get; set; }
        public decimal Total { get; set; }

        public virtual ICollection<OrderDetailDto> OrderDetails { get; set; }
    }
}