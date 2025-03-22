namespace MyMVCFrontend.Models
{
    public class PostWeatherReq
    {
        public DateTime Date { get; set; }
        public int TemperatureC { get; set; }
        public string Summary { get; set; }
    }
}
