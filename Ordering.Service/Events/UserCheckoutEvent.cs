using System;
using System.Collections.Generic;
using Ordering.Domain.AggregateModels.BuyerAggregate;
using Ordering.Domain.AggregateModels.OrderAggregate;

namespace Ordering.API.Events
{
    public class UserCheckoutEvent
    {
        public UserCheckoutEvent()
        {
            OrderDetails = new List<OrderDetail>();
        }

        public string BasketId { get; set; }
        public string OrderId { get; set; }
        public int CustomerId { get; set; }
        public DateTime OrderDate { get; set; }
        public string UserName { get; set; }
        public decimal Total { get; set; }
        public BuyerInformation BuyerInformation { get; set; }
        public List<OrderDetail> OrderDetails { get; set; }
    }
}