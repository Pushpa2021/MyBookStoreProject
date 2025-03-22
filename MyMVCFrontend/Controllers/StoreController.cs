using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using MyMVCFrontend.Models;
using Microsoft.Extensions.Configuration;
using System;

namespace MyMVCFrontend.Controllers
{
    public class StoreController : Controller
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiBaseUrl;

        public StoreController(IHttpClientFactory httpClientFactory, IConfiguration configuration)
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

        public async Task<IActionResult> Index(string searchString, string minPrice, string maxPrice)
        {
            try
            {
                // Construct API URL with query parameters
                string apiUrl = "api/Store/GetBooks";

                var queryParams = new List<string>();
                if (!string.IsNullOrEmpty(searchString))
                    queryParams.Add($"searchString={Uri.EscapeDataString(searchString)}");
                if (!string.IsNullOrEmpty(minPrice))
                    queryParams.Add($"minPrice={minPrice}");
                if (!string.IsNullOrEmpty(maxPrice))
                    queryParams.Add($"maxPrice={maxPrice}");

                if (queryParams.Count > 0)
                    apiUrl += "?" + string.Join("&", queryParams);

                var response = await _httpClient.GetAsync(apiUrl);
                if (!response.IsSuccessStatusCode)
                    return View(new List<Book>());

                var data = await response.Content.ReadAsStringAsync();
                var books = JsonConvert.DeserializeObject<List<Book>>(data);
                return View(books);
            }
            catch (Exception ex)
            {
                ViewData["ErrorMessage"] = "Error fetching book data: " + ex.Message;
                return View(new List<Book>());
            }
        }

        public async Task<IActionResult> Details(int id)
        {
            try
            {
                var response = await _httpClient.GetAsync($"api/Store/GetBookDetails/{id}");
                if (!response.IsSuccessStatusCode)
                    return NotFound();

                var data = await response.Content.ReadAsStringAsync();
                var book = JsonConvert.DeserializeObject<Book>(data);
                return View(book);
            }
            catch (Exception ex)
            {
                ViewData["ErrorMessage"] = "Error fetching book details: " + ex.Message;
                return View();
            }
        }
    }
}
