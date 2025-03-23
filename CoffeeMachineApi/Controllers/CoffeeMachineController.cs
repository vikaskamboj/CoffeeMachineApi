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
        public async Task<IActionResult> BrewCoffeeAsync([FromQuery] string city = "Melbourne")
        {
            var result = await _coffeeService.BrewCoffeeAsync(city);

            if (!result.success)
            {
                return result.message switch
                {
                    "418" => StatusCode(418, "I'm a teapot"),
                    "503" => StatusCode(503, "Service Unavailable"),
                    _ => StatusCode(500, "Internal Server Error")
                };
            }

            //return Ok(result.message);
            return Ok(new
            {
                message = result.message,
                prepared = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:sszzz")
            });
        }
    }
}
