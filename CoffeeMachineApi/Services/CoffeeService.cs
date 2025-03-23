using CoffeeMachineApi.Data;
using CoffeeMachineApi.Helpers;

namespace CoffeeMachineApi.Services
{
    public class CoffeeService : ICoffeeService
    {
        private readonly CoffeeMachineDbContext _ctx;
        private readonly ILogger<CoffeeService> _logger;
        private readonly IDateTimeProvider _dateTimeProvider;
        private readonly IWeatherService _weatherService;
        private static int _requestCount = 0;

        public CoffeeService(CoffeeMachineDbContext ctx, IDateTimeProvider dateTimeProvider, IWeatherService weatherService, ILogger<CoffeeService> logger)
        { 
            _ctx = ctx;
            _logger = logger;
            _dateTimeProvider = dateTimeProvider;
            _weatherService = weatherService;
        }

        public async Task<(bool success, string message)> BrewCoffeeAsync(string city)
        {
            _requestCount++;

            // Check the current temperature
            var currentTemperature = await _weatherService.GetCurrentTemperatureAsync(city);

            string message = "Your piping hot coffee is ready";
            // If temperature is greater than 30°C, change the message to iced coffee
            if (currentTemperature > 30)
            {
                message = "Your refreshing iced coffee is ready";
            }

            // Check if it's April the 1st
            if (_dateTimeProvider.Now.Month == 4 && _dateTimeProvider.Now.Day == 1)
            {
                _logger.LogWarning("418, I'm a teapot.");
                return (false, "418");
            }

            // Make every 5th request to fail
            if (_requestCount % 5 == 0)
            {
                _logger.LogWarning("503, Service Unavailable.");
                return (false, "503");
            }

            var brew = new CoffeeBrew
            {
                BrewTime = _dateTimeProvider.Now,
                Status = "Ready"
            };

            _ctx.Brews.Add(brew);
            await _ctx.SaveChangesAsync();

            _logger.LogInformation("Coffee brewed successfully at {Time}.", _dateTimeProvider.Now);
            return (true, message);
        }
    }
}
