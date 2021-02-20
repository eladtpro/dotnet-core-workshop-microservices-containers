using System;
using System.Collections.Generic;
using System.Linq;
using Ordering.Domain.Contracts;

namespace Ordering.Domain.AggregateModels.OrderAggregate
{
    public class Order : IAggregateRoot
    {
        public IReadOnlyCollection<OrderDetail> OrderDetails;

        public Order(
            string orderId,
            int customerId,
            string basketId,
            DateTime orderdate,
            string buyerId,
            decimal total,
            IReadOnlyCollection<OrderDetail> orderDetails)
        {
            OrderId = orderId;
            CustomerId = customerId;
            BasketId = basketId;
            BuyerId = buyerId;
            OrderDate = orderdate;
            BuyerId = buyerId;
            Total = total;
            OrderDetails = orderDetails;
        }

        // DDD Patterns comment
        // Using private fields to encapsulate and carefully manage data.
        // The only way to create a Buyer is through the constructor enabling
        // the domain class to enforce business rules and validation
        public string OrderId { get; private set; }
        public int CustomerId { get; private set; }
        public string BasketId { get; set; }
        public DateTime OrderDate { get; private set; }
        public string BuyerId { get; private set; }
        public decimal Total { get; private set; }

        // DDD Patterns comment
        // Any behavior(discounts, etc.) and validations are controlled by the Aggregate Root
        // in order to maintain consistency across the whole Aggregate.
        // This Order AggregateRoot's method "GetLineItemCount" is the only way to obtain a count
        // of items for an Order.;   
        public int GetLineItemCount()
        {
            return OrderDetails.Count;
        }

        // This Order AggregateRoot's method "GetOrderTotalPrice" is the only way to obtain
        // the price of the order.  
        public decimal GetOrderTotalPrice()
        {
            return Total;
        }

        // This Order AggregateRoot's method "ApplyDiscount" is the only way to apply a discount
        // to the order.  
        public decimal ApplyDiscount(decimal discount)
        {
            return Total * discount;
        }

        // This Order AggregateRoot's method "ProductsBackOrdered" is the only way to identity the 
        // number of products backorderd for an order.  
        public int ProductsBackOrdered()
        {
            return OrderDetails.Count(x => x.Backordered);
        }

        // This Order AggregateRoot's method "GetUnitsForOrder" is the only way to return the number
        // of units for an order.  
        public int GetUnitsForOrder()
        {
            return OrderDetails.Sum(x => x.Quantity);
        }

        // This Order AggregateRoot's method "GetOrderDetailsForOrder" is the only way to return the number
        // of fetch line item details for an order.  
        public List<OrderDetail> GetOrderDetailsForOrder(int orderId)
        {
            throw new NotImplementedException();
        }

        // This Order AggregateRoot's method "GetMostExpensvieLineItemForOrder" is the only way to 
        // return the highest priced line item for an order
        public List<OrderDetail> GetMostExpensvieLineItemForOrder(int orderId)
        {
            throw new NotImplementedException();
        }
    }
}