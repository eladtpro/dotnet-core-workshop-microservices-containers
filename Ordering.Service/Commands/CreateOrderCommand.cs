using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Ordering.Domain.AggregateModels.OrderAggregate;

namespace Ordering.API.Commands
{
    [DataContract]
    public class CreateOrderCommand
    {
        public IReadOnlyCollection<OrderDetail> OrderDetails;

        public CreateOrderCommand(
            int customerId,
            string basketId,
            DateTime orderdate,
            string userName,
            decimal total,
            List<OrderDetail> orderDetails)
        {
            CustomerId = customerId;
            BasketId = basketId;
            OrderDate = orderdate;
            BuyerId = userName;
            Total = total;
            OrderDetails = orderDetails;
        }

        public int CustomerId { get; }
        public string BasketId { get; }
        public DateTime OrderDate { get; }
        public string BuyerId { get; }
        public decimal Total { get; }
    }
}