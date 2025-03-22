using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MyMVCFrontend.Models
{
    public class Cart
    {
        public string Id { get; set; }
        public List<CartItem> CartItems { get; set; }
    }
}
