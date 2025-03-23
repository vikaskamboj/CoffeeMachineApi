using System.Text.Json;
using System.Text.Json.Serialization;

namespace CoffeeMachineApi.Services
{
    public class WeatherService : IWeatherService
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiKey = "bd5e378503939ddaee76f12ad7a97608"; 

        public WeatherService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<double> GetCurrentTemperatureAsync(string city)
        {
            var response = await _httpClient.GetAsync($"http://api.openweathermap.org/data/2.5/weather?q={city}&appid={_apiKey}&units=metric");
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();
            var weatherResponse = JsonSerializer.Deserialize<WeatherResponse>(content);

            return weatherResponse?.Main?.Temp ?? 0;
        }
    }

    public class WeatherResponse
    {
        [JsonPropertyName("main")]
        public Main Main { get; set; }
    }

    public class Main
    {
        [JsonPropertyName("temp")]
        public double Temp { get; set; }
    }

}
