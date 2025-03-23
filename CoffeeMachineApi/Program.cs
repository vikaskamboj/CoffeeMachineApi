using AspNetCoreRateLimit;
using CoffeeMachineApi.Data;
using CoffeeMachineApi.Helpers;
using CoffeeMachineApi.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using System.Text;

namespace CoffeeMachineApi
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
            builder.Logging.AddConsole();

            builder.Services
                .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = builder.Configuration["Jwt:Issuer"],
                        ValidAudience = builder.Configuration["Jwt:Audience"],
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
                    };
                });

            builder.Services.AddAuthorization();

            builder.Host.UseSerilog((context, config) => config.WriteTo.Console());

            // Use In-Memory database for all environments
            builder.Services.AddDbContext<CoffeeMachineDbContext>(options =>
                options.UseInMemoryDatabase("CoffeeMachineDb"));

            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowAll",
                    policy => policy.AllowAnyOrigin()
                                    .AllowAnyMethod()
                                    .AllowAnyHeader());
            });

            builder.Services.AddMemoryCache();
            builder.Services.Configure<IpRateLimitOptions>(options =>
            {
                options.GeneralRules = new List<RateLimitRule>
                {
                    new RateLimitRule
                    {
                        Endpoint = "*",
                        Limit = 10, // Max 10 requests
                        Period = "1m" // Per 1 minute
                    }
                };
            });
            builder.Services.AddSingleton<IRateLimitConfiguration, RateLimitConfiguration>();
            builder.Services.AddInMemoryRateLimiting();
            
            builder.Services.AddScoped<ICoffeeService, CoffeeService>();
            builder.Services.AddScoped<IDateTimeProvider, DateTimeProvider>();

            var app = builder.Build();
            app.UseCors("AllowAll");

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseMiddleware<CoffeeMachineApi.Middleware.ExceptionHandlingMiddleware>();
            app.UseHttpsRedirection();
            app.UseSerilogRequestLogging();

            if (!app.Environment.IsDevelopment()) // Disable authentication in dev mode
            {
                app.UseAuthentication();
                app.UseAuthorization();
            }

            app.MapControllers();
            Console.WriteLine($"Environment: {app.Environment.EnvironmentName}");
            app.Run();
        }
    }
}
