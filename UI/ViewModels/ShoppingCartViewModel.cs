﻿using System.Collections.Generic;
using MusicStore.Models;

namespace MusicStore.ViewModels
{
    public class ShoppingCartViewModel
    {
        public List<BasketItemDto> CartItems { get; set; }
        public decimal CartTotal { get; set; }
    }
}
