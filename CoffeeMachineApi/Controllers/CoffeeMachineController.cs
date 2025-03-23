using CoffeeMachineApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CoffeeMachineApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CoffeeMachineController : Controller
    {
        private readonly ICoffeeService _coffeeService;

        public CoffeeMachineController(ICoffeeService coffeeService)
        {
            _coffeeService = coffeeService;
        }

        [HttpGet("brew-coffee")]
        [Authorize]
        public async Task<IActionResult> BrewCoffee()
        {
            var (success, message) = await _coffeeService.BrewCoffeeAsync();

            if (message == "418")
                return StatusCode(418); // I'm a teapot (April 1st)

            if (message == "503")
                return StatusCode(503); // Service Unavailable (Every 5th request)

            return Ok(new
            {
                message,
                prepared = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:sszzz")
            });
        }
    }
}
