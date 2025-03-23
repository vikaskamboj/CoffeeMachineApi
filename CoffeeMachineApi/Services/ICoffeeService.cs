using System.Threading.Tasks;

namespace CoffeeMachineApi.Services
{
    public interface ICoffeeService
    {
        Task<(bool success, string message)> BrewCoffeeAsync();
    }
}
