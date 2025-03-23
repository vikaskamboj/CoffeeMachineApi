using CoffeeMachineApi.Data;
using CoffeeMachineApi.Helpers;
using CoffeeMachineApi.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;

namespace CoffeeMachineApiTests.Controllers
{
    public class CoffeeMachineControllerTests
    {
        private readonly Mock<ILogger<CoffeeService>> _mockLogger;
        private readonly CoffeeMachineDbContext _dbContext;
        private readonly Mock<IDateTimeProvider> _mockDateTimeProvider;
        private readonly Mock<IWeatherService> _mockWeatherService;
        private readonly ICoffeeService _coffeeService;

        public CoffeeMachineControllerTests()
        {
            // Set up an in-memory database
            var options = new DbContextOptionsBuilder<CoffeeMachineDbContext>()
                .UseInMemoryDatabase(databaseName: "CoffeeTestDb")
                .Options;

            _dbContext = new CoffeeMachineDbContext(options);
            _mockLogger = new Mock<ILogger<CoffeeService>>();
            _mockDateTimeProvider = new Mock<IDateTimeProvider>();
            _mockWeatherService = new Mock<IWeatherService>();
            _coffeeService = new CoffeeService(_dbContext, _mockDateTimeProvider.Object, _mockWeatherService.Object, _mockLogger.Object);
        }

        [Fact]
        public async Task BrewCoffeeAsync_ShouldReturnSuccess_WhenBrewingCoffee()
        {
            // Arrange
            var expectedMessage = "Your piping hot coffee is ready";

            // Act
            var result = await _coffeeService.BrewCoffeeAsync("");

            // Assert
            Assert.True(result.success);
            Assert.Equal(expectedMessage, result.message);
        }

        [Fact]
        public async Task BrewCoffeeAsync_ShouldReturn503_WhenFifthRequest()
        {
            // Arrange
            // Simulate 4 previous requests
            for (int i = 0; i < 4; i++)
            {
                await _coffeeService.BrewCoffeeAsync("");
            }

            // Act
            var result = await _coffeeService.BrewCoffeeAsync("");

            // Assert
            Assert.False(result.success);
            Assert.Equal("503", result.message);
        }

        [Fact]
        public async Task BrewCoffeeAsync_ShouldReturn418_WhenApril1st()
        {
            // Arrange: Set current date to April 1st
            _mockDateTimeProvider.Setup(m => m.Now).Returns(new DateTime(2023, 4, 1)); // April 1st

            // Act
            var result = await _coffeeService.BrewCoffeeAsync("");

            // Assert
            Assert.False(result.success);
            Assert.Equal("418", result.message);
        }
    }
}