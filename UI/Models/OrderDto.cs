using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MusicStore.Models
{
    public class OrderDto
    {
        [DisplayName("Order Id")]
        public string OrderId { get; set; }
        [DisplayName("Shopping Cart Id")]
        public string ShoppingBasketId { get; set; }
        [DisplayName("Order Date")]
        public DateTime OrderDate { get; set; }
        [DisplayName("User")]
        [StringLength(25)]
        public string Username { get; set; }
        [DataType(DataType.Currency)]
        public decimal Total { get; set; }

    }
}
