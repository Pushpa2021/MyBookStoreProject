using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using MyMVCFrontend.Models;
using Microsoft.Extensions.Configuration;

namespace BookStore.Web.Controllers
{
    public class BooksController : Controller
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiBaseUrl;

        public BooksController(IHttpClientFactory httpClientFactory, IConfiguration configuration)
        {
            _httpClient = httpClientFactory.CreateClient();
            _apiBaseUrl = configuration["ApiSettings:BaseUrl"];

            if (string.IsNullOrEmpty(_apiBaseUrl))
            {
                throw new InvalidOperationException("API Base URL is not set in configuration.");
            }

            _httpClient.BaseAddress = new Uri(_apiBaseUrl);
            Console.WriteLine(_apiBaseUrl);
        }

        public async Task<IActionResult> Index()
        {
            var response = await _httpClient.GetAsync("api/BooksApi");
            if (!response.IsSuccessStatusCode)
                return View(new List<Book>());

            var data = await response.Content.ReadAsStringAsync();
            var books = JsonConvert.DeserializeObject<List<Book>>(data);
            return View(books);
        }

        public async Task<IActionResult> Details(int id)
        {
            var response = await _httpClient.GetAsync($"api/BooksApi/{id}");
            if (!response.IsSuccessStatusCode)
                return NotFound();

            var data = await response.Content.ReadAsStringAsync();
            var book = JsonConvert.DeserializeObject<Book>(data);
            return View(book);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Book book)
        {
            if (!ModelState.IsValid)
                return View(book);

            var jsonData = JsonConvert.SerializeObject(book);
            var content = new StringContent(jsonData, Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync("api/BooksApi", content);

            if (response.IsSuccessStatusCode)
                return RedirectToAction(nameof(Index));

            ModelState.AddModelError(string.Empty, "Failed to create book.");
            return View(book);
        }

        public async Task<IActionResult> Edit(int id)
        {
            var response = await _httpClient.GetAsync($"api/BooksApi/{id}");
            if (!response.IsSuccessStatusCode)
                return NotFound();

            var data = await response.Content.ReadAsStringAsync();
            var book = JsonConvert.DeserializeObject<Book>(data);
            return View(book);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Book book)
        {
            if (id != book.Id)
                return BadRequest();

            if (!ModelState.IsValid)
                return View(book);

            var jsonData = JsonConvert.SerializeObject(book);
            var content = new StringContent(jsonData, Encoding.UTF8, "application/json");
            var response = await _httpClient.PutAsync($"api/BooksApi/{id}", content);

            if (response.IsSuccessStatusCode)
                return RedirectToAction(nameof(Index));

            ModelState.AddModelError(string.Empty, "Failed to update book.");
            return View(book);
        }

        public async Task<IActionResult> Delete(int id)
        {
            var response = await _httpClient.GetAsync($"api/BooksApi/{id}");
            if (!response.IsSuccessStatusCode)
                return NotFound();

            var data = await response.Content.ReadAsStringAsync();
            var book = JsonConvert.DeserializeObject<Book>(data);
            return View(book);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var response = await _httpClient.DeleteAsync($"api/BooksApi/{id}");

            if (response.IsSuccessStatusCode)
                return RedirectToAction(nameof(Index));

            return NotFound();
        }
    }
}
