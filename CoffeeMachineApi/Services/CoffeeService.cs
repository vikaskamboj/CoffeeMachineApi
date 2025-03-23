using CoffeeMachineApi.Data;
using CoffeeMachineApi.Helpers;

namespace CoffeeMachineApi.Services
{
    public class CoffeeService : ICoffeeService
    {
        private readonly CoffeeMachineDbContext _ctx;
        private readonly ILogger<CoffeeService> _logger;
        private readonly IDateTimeProvider _dateTimeProvider;
        private static int _requestCount = 0;

        public CoffeeService(CoffeeMachineDbContext ctx, IDateTimeProvider dateTimeProvider, ILogger<CoffeeService> logger)
        { 
            _ctx = ctx;
            _logger = logger;
            _dateTimeProvider = dateTimeProvider;
        }

        public async Task<(bool success, string message)> BrewCoffeeAsync()
        {
            _requestCount++;

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
            return (true, "Your piping hot coffee is ready");
        }
    }
}
