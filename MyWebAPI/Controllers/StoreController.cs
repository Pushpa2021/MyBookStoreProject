using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyWebAPI.Data;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace MyWebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StoreController : ControllerBase
    {
        private readonly BookStoreContext _context;

        public StoreController(BookStoreContext context)
        {
            _context = context;
        }

        [HttpGet("GetBooks")]
        public async Task<IActionResult> GetBooks([FromQuery] string? searchString, [FromQuery] decimal? minPrice, [FromQuery] decimal? maxPrice)
        {
            var books = _context.Books.AsQueryable();

            if (!string.IsNullOrEmpty(searchString))
            {
                books = books.Where(b => b.Title.Contains(searchString) || b.Author.Contains(searchString));
            }

            if (minPrice.HasValue)
            {
                books = books.Where(b => b.Price >= minPrice.Value);
            }

            if (maxPrice.HasValue)
            {
                books = books.Where(b => b.Price <= maxPrice.Value);
            }

            return Ok(await books.ToListAsync());
        }

        [HttpGet("GetBookDetails/{id}")]
        public async Task<IActionResult> GetBookDetails(int id)
        {
            var book = await _context.Books.FirstOrDefaultAsync(m => m.Id == id);
            if (book == null)
            {
                return NotFound(new { message = "Book not found" });
            }

            return Ok(book);
        }
    }
}
