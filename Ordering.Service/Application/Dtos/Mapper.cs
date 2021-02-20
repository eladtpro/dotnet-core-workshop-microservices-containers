using System.Collections.Generic;

namespace Ordering.API.Application.Dtos
{
    public class Mapper
    {
        public static OrderDto MapToOrderDto(dynamic order)
        {
            var orderDto = new OrderDto
            {
                OrderId = order.Id,
                OrderDate = order.OrderDate,
                ShoppingBasketId = order.BasketId,

                // Must add ToString() in order to parse the decimal, otherwise it errors
                Total = decimal.Parse(order.Total?.ToString()),
                Username = order.BuyerId
            };

            foreach (var item in order.OrderDetails)
                orderDto.OrderDetails.Add(new OrderDetailDto
                {
                    //OrderDetailId = item.OrderDetailId,
                    AlbumId = item.AlbumId,
                    OrderId = order.Id,
                    Quantity = item.Quantity,
                    UnitPrice = decimal.Parse(item.UnitPrice?.ToString()),
                    Artist = item.Artist,
                    Title = item.Item
                });

            return orderDto;
        }

        public static List<FlattenedOrderDto> MapToOrdersDto(List<dynamic> orders)
        {
            var orderDtos = new List<FlattenedOrderDto>();

            foreach (var order in orders)
                orderDtos.Add(new FlattenedOrderDto
                {
                    OrderId = order.id,
                    ShoppingBasketId = order.BasketId,
                    OrderDate = order.OrderDate,
                    Total = decimal.Parse(order.Total?.ToString())
                });

            return orderDtos;
        }
    }
}