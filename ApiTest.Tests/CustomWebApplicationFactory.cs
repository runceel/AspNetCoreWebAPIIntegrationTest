using ApiTest.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using System.Security.Cryptography;

namespace ApiTest.Tests
{
    public class CustomWebApplicationFactory<TStartup> : WebApplicationFactory<TStartup>
        where TStartup: class
    {
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureServices(services =>
            {
                // DB を SQL Server からインメモリーにする
                var descriptor = services.SingleOrDefault(
                    x => x.ServiceType == typeof(DbContextOptions<WeatherContext>));
                if (descriptor != null)
                {
                    services.Remove(descriptor);
                }
                services.AddDbContext<WeatherContext>(options =>
                {
                    options.UseInMemoryDatabase("Testing");
                });

                var sp = services.BuildServiceProvider();
                // Scope を作っておくことで DbContext が使いまわされないようにする
                using (var scope = sp.CreateScope())
                {
                    var db = scope.ServiceProvider.GetRequiredService<WeatherContext>();

                    // DB を作り直し
                    db.Database.EnsureDeleted();
                    db.Database.EnsureCreated();
                    // テストデータの投入
                    db.WeatherForecasts.AddRange(new WeatherForecast
                    {
                        City = "Tokyo",
                        Summary = "Cold",
                        Date = new DateTime(2020, 1, 1),
                        TemperatureC = 0,
                    },
                    new WeatherForecast
                    {
                        City = "Tokyo",
                        Summary = "Hot",
                        Date = new DateTime(2020, 8, 6),
                        TemperatureC = 35,
                    },
                    new WeatherForecast
                    {
                        City = "Hiroshima",
                        Summary = "Cold",
                        Date = new DateTime(2020, 1, 1),
                        TemperatureC = -1,
                    },
                    new WeatherForecast
                    {
                        City = "Hiroshima",
                        Summary = "Hot",
                        Date = new DateTime(2020, 8, 6),
                        TemperatureC = 32,
                    });
                    db.SaveChanges();
                }
            });
        }
    }
}
