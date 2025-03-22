using Microsoft.AspNetCore.Mvc;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using MyWebAPI.Data;
using MyWebAPI.Models;

namespace MyWebAPI.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class CartApiController : ControllerBase
    {
        private readonly BookStoreContext _context;
        private readonly Cart _cart;

        public CartApiController(BookStoreContext context, Cart cart)
        {
            _context = context;
            _cart = cart;
        }

        [HttpGet]
        public IActionResult GetCart()
        {
            var items = _cart.GetAllCartItems();
            _cart.CartItems = items;
            return Ok(_cart);
        }

        [HttpPost("AddToCart/{id}")]
        public IActionResult AddToCart(int id)
        {
            var selectedBook = GetBookById(id);
            if (selectedBook != null)
                _cart.AddToCart(selectedBook, 1);
            return Ok();
        }

        [HttpDelete("RemoveFromCart/{id}")]
        public IActionResult RemoveFromCart(int id)
        {
            var selectedBook = GetBookById(id);
            if (selectedBook != null)
                _cart.RemoveFromCart(selectedBook);
            return Ok();
        }

        [HttpPost("ReduceQuantity/{id}")]
        public IActionResult ReduceQuantity(int id)
        {
            var selectedBook = GetBookById(id);
            if (selectedBook != null)
                _cart.ReduceQuantity(selectedBook);
            return Ok();
        }

        [HttpPost("IncreaseQuantity/{id}")]
        public IActionResult IncreaseQuantity(int id)
        {
            var selectedBook = GetBookById(id);
            if (selectedBook != null)
                _cart.IncreaseQuantity(selectedBook);
            return Ok();
        }

        [HttpPost("ClearCart")]
        public IActionResult ClearCart()
        {
            _cart.ClearCart();
            return Ok();
        }

        private Book GetBookById(int id)
        {
            return _context.Books.FirstOrDefault(b => b.Id == id);
        }
    }
}