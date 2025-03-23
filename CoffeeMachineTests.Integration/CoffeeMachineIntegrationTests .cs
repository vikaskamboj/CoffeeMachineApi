using CoffeeMachineApi;
using CoffeeMachineApi.Data;
using CoffeeMachineApi.Helpers;
using CoffeeMachineApi.Services;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;

namespace CoffeeMachineTests.Integration
{
    public class CoffeeMachineIntegrationTests : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly HttpClient _client;
        private readonly WebApplicationFactory<Program> _factory;

        public CoffeeMachineIntegrationTests(WebApplicationFactory<Program> factory)
        {
            _factory = factory.WithWebHostBuilder(builder =>
            {
                // Clear any existing database configurations (both SQLServer and InMemory) to avoid conflicts
                builder.ConfigureServices(services =>
                {
                    // Add InMemory database for testing.
                    services.AddDbContext<CoffeeMachineDbContext>(options =>
                        options.UseInMemoryDatabase("CoffeeTestDb")); // InMemory database
                });
            });

            // Create an HTTP client to interact with the API
            _client = _factory.CreateClient();
        }

        [Fact]
        public async Task BrewCoffeeAsync_ShouldReturnSuccess_WhenBrewingCoffee()
        {
            // Arrange: Ensure the database is cleared and created in the scope
            using (var scope = _factory.Services.CreateScope())
            {
                var serviceProvider = scope.ServiceProvider;
                var context = serviceProvider.GetRequiredService<CoffeeMachineDbContext>();

                context.Database.EnsureDeleted();
                context.Database.EnsureCreated();

                var coffeeService = new CoffeeService(context, Mock.Of<IDateTimeProvider>(), Mock.Of<ILogger<CoffeeService>>());

                _client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJodHRwOi8vc2NoZW1hcy54bWxzb2FwLm9yZy93cy8yMDA1LzA1L2lkZW50aXR5L2NsYWltcy9uYW1lIjoiQ29mZmVlVXNlciIsImV4cCI6MTc0Mjc0NzcxMiwiaXNzIjoiQ29mZmVlTWFjaGluZUFQSSIsImF1ZCI6IkNvZmZlZU1hY2hpbmVDbGllbnQifQ.wIeVqwCq_eW6rfQZMF2pUpGoQKHCo-LgD01OQDUN3OU");

                // Act: Send the request to the API endpoint
                var response = await _client.GetAsync("/api/CoffeeMachine/brew-coffee");

                // Assert: Check the response status and the message
                response.EnsureSuccessStatusCode();
                var content = await response.Content.ReadAsStringAsync();
                Assert.Contains("Your piping hot coffee is ready", content);
            }
        }

        [Fact]
        public async Task BrewCoffeeAsync_ShouldReturn503_WhenFifthRequest()
        {
            // Arrange: Ensure the database is cleared and created in the scope
            using (var scope = _factory.Services.CreateScope())
            {
                var serviceProvider = scope.ServiceProvider;
                var context = serviceProvider.GetRequiredService<CoffeeMachineDbContext>();

                context.Database.EnsureDeleted();
                context.Database.EnsureCreated();


                var coffeeService = new CoffeeService(context, Mock.Of<IDateTimeProvider>(), Mock.Of<ILogger<CoffeeService>>());

                // Simulate 4 previous requests
                for (int i = 0; i < 4; i++)
                {
                    await coffeeService.BrewCoffeeAsync();
                }

                _client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJodHRwOi8vc2NoZW1hcy54bWxzb2FwLm9yZy93cy8yMDA1LzA1L2lkZW50aXR5L2NsYWltcy9uYW1lIjoiQ29mZmVlVXNlciIsImV4cCI6MTc0Mjc0NzcxMiwiaXNzIjoiQ29mZmVlTWFjaGluZUFQSSIsImF1ZCI6IkNvZmZlZU1hY2hpbmVDbGllbnQifQ.wIeVqwCq_eW6rfQZMF2pUpGoQKHCo-LgD01OQDUN3OU");

                // Act: Send the fifth request to trigger failure
                var response = await _client.GetAsync("/api/CoffeeMachine/brew-coffee");

                // Assert: Ensure the response is a failure (503)
                var content = await response.Content.ReadAsStringAsync();
                Assert.Contains("503", content);
            }
        }
    }
}