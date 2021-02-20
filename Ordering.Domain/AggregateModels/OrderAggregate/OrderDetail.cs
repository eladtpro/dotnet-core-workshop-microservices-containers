using System;
using System.Collections.Generic;
using Ordering.Domain.Exceptions;

namespace Ordering.Domain.AggregateModels.OrderAggregate
{
    public class OrderDetail
    {
        protected OrderDetail()
        {
        }

        public OrderDetail(string name,
            string artist,
            int albumId,
            int quantity,
            decimal unitPrice,
            string item)
        {
            if (quantity <= 0)
                Backordered = true;
            else
                Backordered = false;

            if (unitPrice < 0m)
                throw new OrderingDomainException("Price cannot be zero");

            Name = name;
            Artist = artist;
            AlbumId = albumId;
            Quantity = quantity;
            UnitPrice = unitPrice;
            Item = item;
        }

        //public int OrderDetailId { get; set; }
        //public int OrderId { get; set; }
        public string Name { get; }

        public bool Backordered { get; set; }
        public string Artist { get; }
        public int AlbumId { get; }
        public int Quantity { get; }
        public decimal UnitPrice { get; }
        public string Item { get; }

        public List<OrderDetail> GetOrderDetailsForOrder(int orderId)
        {
            throw new NotImplementedException(); 
        }
    }
}