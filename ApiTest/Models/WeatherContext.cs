using Microsoft.EntityFrameworkCore;
using System;

namespace ApiTest.Models
{
    public class WeatherContext : DbContext
    {
        public DbSet<WeatherForecast> WeatherForecasts { get; set; }

        public WeatherContext()
        {
        }

        public WeatherContext(DbContextOptions<WeatherContext> options) : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<WeatherForecast>(b =>
            {
                b.Property(x => x.Id);
                b.HasKey(x => x.Id);
                b.Property(x => x.City).IsRequired();
                b.Property(x => x.TemperatureC).IsRequired();
                b.Property(x => x.Date).IsRequired();
                b.Property(x => x.Summary).IsRequired();
            });
        }
    }

    public class WeatherForecast
    {
        public int Id { get; set; }
        public string City { get; set; }
        public int TemperatureC { get; set; }
        public DateTime Date { get; set; }
        public string Summary { get; set; }
    }
}
