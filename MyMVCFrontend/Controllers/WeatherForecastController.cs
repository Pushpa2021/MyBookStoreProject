using Microsoft.AspNetCore.Mvc;
using MyMVCFrontend.Models;
using Newtonsoft.Json;
using System.Net.Http.Json;
using System.Text.Json.Serialization;

namespace MyMVCFrontend.Controllers
{
    using Microsoft.AspNetCore.Mvc;
    using Newtonsoft.Json;
    using System.Net.Http;
    using System.Text;
    using System.Threading.Tasks;

    public class WeatherForecastController : Controller
    {
        private readonly HttpClient _httpClient;

        public WeatherForecastController(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient("WeatherAPI");
        }

        public async Task<IActionResult> Index()
        {
            List<WeatherForecast> forecasts = new();
            HttpResponseMessage response = await _httpClient.GetAsync("weatherforecast");

            if (response.IsSuccessStatusCode)
            {
                string data = await response.Content.ReadAsStringAsync();
                forecasts = JsonConvert.DeserializeObject<List<WeatherForecast>>(data);
            }

            return View(forecasts);
        }
        public async Task<IActionResult> Create()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Create(PostWeatherReq forecast)
        {
            if (forecast == null)
            {
                return BadRequest("Invalid data");
            }

            // ✅ Convert object to JSON
            string jsonData = JsonConvert.SerializeObject(forecast);
            StringContent content = new StringContent(jsonData, Encoding.UTF8, "application/json");

            // ✅ Send POST request
            HttpResponseMessage response = await _httpClient.PostAsync("weatherforecast", content);

            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction("Index"); // Redirect after successful POST
            }
            else
            {
                return StatusCode((int)response.StatusCode, "Error in API request");
            }
        }
    }

}
