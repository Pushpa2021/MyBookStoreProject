using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using MyMVCFrontend.Models;
using System.Text;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;

namespace MyMVCFrontend.Controllers
{
    public class CartController : Controller
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiBaseUrl;

        public CartController(IHttpClientFactory httpClientFactory, IConfiguration configuration)
        {
            _httpClient = httpClientFactory.CreateClient();
            _apiBaseUrl = configuration["ApiSettings:BaseUrl"];
        }

        public async Task<IActionResult> Index()
        {
            var response = await _httpClient.GetAsync($"{_apiBaseUrl}/api/CartApi");
            if (!response.IsSuccessStatusCode)
                return View(new List<CartItem>());

            var data = await response.Content.ReadAsStringAsync();
            var cartItems = JsonConvert.DeserializeObject<List<CartItem>>(data);
            return View(cartItems);
        }

        public async Task<IActionResult> AddToCart(int id)
        {
            var response = await _httpClient.PostAsync($"{_apiBaseUrl}/api/CartApi/AddToCart/{id}", null);
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> RemoveFromCart(int id)
        {
            var response = await _httpClient.PostAsync($"{_apiBaseUrl}/api/CartApi/RemoveFromCart/{id}", null);
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> ClearCart()
        {
            var response = await _httpClient.PostAsync($"{_apiBaseUrl}/api/CartApi/ClearCart", null);
            return RedirectToAction("Index");
        }
    }
}
