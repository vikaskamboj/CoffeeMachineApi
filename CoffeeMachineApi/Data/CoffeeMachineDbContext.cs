using Microsoft.EntityFrameworkCore;

namespace CoffeeMachineApi.Data
{
    public class CoffeeMachineDbContext : DbContext
    {
        public CoffeeMachineDbContext(DbContextOptions<CoffeeMachineDbContext> options): base(options) { }

        public DbSet<CoffeeBrew> Brews { get; set; }
    }

    public class CoffeeBrew
    {
        public int Id { get; set; }
        public DateTime BrewTime { get; set; }
        public string Status { get; set; }
    }
}