using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ApiTest.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace ApiTest.Controllers
{
    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private readonly ILogger<WeatherForecastController> _logger;
        private readonly WeatherContext _weatherContext;

        public WeatherForecastController(ILogger<WeatherForecastController> logger, WeatherContext weatherContext)
        {
            _logger = logger;
            _weatherContext = weatherContext;
        }

        [HttpGet]
        public async Task<IEnumerable<WeatherForecastResponse>> Get([FromQuery]string city)
        {
            _logger.LogDebug($"Get weather forecasts for {city}");
            IQueryable<WeatherForecast> query = _weatherContext.WeatherForecasts;
            if (!string.IsNullOrWhiteSpace(city))
            {
                query = query.Where(x => x.City == city);
            }

            var forecasts = await query.ToArrayAsync();
            return forecasts.Select(x => new WeatherForecastResponse
            {
                City = x.City,
                Date = x.Date,
                Summary = x.Summary,
                TemperatureC = x.TemperatureC,
            });
        }
    }
}
