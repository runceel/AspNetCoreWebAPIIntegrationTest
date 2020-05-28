using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;
using Xunit;

namespace ApiTest.Tests.Controllers
{
    public class WeatherForecastControllerTest : IClassFixture<CustomWebApplicationFactory<Startup>>
    {
        private readonly CustomWebApplicationFactory<Startup> _factory;

        public WeatherForecastControllerTest(CustomWebApplicationFactory<Startup> factory)
        {
            _factory = factory;
        }

        [Fact]
        public async Task Unauthorized()
        {
            var client = _factory.CreateClient(new WebApplicationFactoryClientOptions
            {
                AllowAutoRedirect = false,
            });
            var forecasts = await client.GetAsync("/WeatherForecast");
            forecasts.StatusCode.Is(HttpStatusCode.Unauthorized);
        }

        [Fact]
        public async Task GetAllForecasts()
        {
            var client = _factory.WithWebHostBuilder(b =>
                {
                    // テスト用の認証ハンドラーを設定する
                    b.ConfigureTestServices(services =>
                    {
                        services.AddAuthentication("Test")
                            .AddScheme<AuthenticationSchemeOptions, TestAuthHandler>(
                                "Test", options => { });
                    });
                })
                .CreateClient(new WebApplicationFactoryClientOptions
                {
                    AllowAutoRedirect = false,
                });

            var res = await client.GetAsync("/WeatherForecast");
            res.StatusCode.Is(HttpStatusCode.OK);

            var responseContent = await JsonSerializer.DeserializeAsync<WeatherForecastResponse[]>(
                await res.Content.ReadAsStreamAsync(),
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            // サーバー側では、順序保障してないのでローカルでソートしてアサート
            responseContent = responseContent.OrderBy(x => x.TemperatureC).ToArray();
            responseContent.Is(
                new[]
                {
                    new WeatherForecastResponse { City = "Hiroshima", Summary = "Cold", Date = new DateTime(2020, 1, 1), TemperatureC = -1 },
                    new WeatherForecastResponse { City = "Tokyo", Summary = "Cold", Date = new DateTime(2020, 1, 1), TemperatureC = 0 },
                    new WeatherForecastResponse { City = "Hiroshima", Summary = "Hot", Date = new DateTime(2020, 8, 6), TemperatureC = 32 },
                    new WeatherForecastResponse { City = "Tokyo", Summary = "Hot", Date = new DateTime(2020, 8, 6), TemperatureC = 35 },
                },
                (x, y) => x.City == y.City &&
                    x.Date == y.Date &&
                    x.Summary == y.Summary &&
                    x.TemperatureC == y.TemperatureC &&
                    x.TemperatureF == y.TemperatureF);
        }

        [Fact]
        public async Task GetTokyoForecasts()
        {
            var client = _factory.WithWebHostBuilder(b =>
                {
                    // テスト用の認証ハンドラーを設定する
                    b.ConfigureTestServices(services =>
                    {
                        services.AddAuthentication("Test")
                            .AddScheme<AuthenticationSchemeOptions, TestAuthHandler>(
                                "Test", options => { });
                    });
                })
                .CreateClient(new WebApplicationFactoryClientOptions
                {
                    AllowAutoRedirect = false,
                });

            var res = await client.GetAsync("/WeatherForecast?city=Tokyo");
            res.StatusCode.Is(HttpStatusCode.OK);

            var responseContent = await JsonSerializer.DeserializeAsync<WeatherForecastResponse[]>(
                await res.Content.ReadAsStreamAsync(),
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            // サーバー側では、順序保障してないのでローカルでソートしてアサート
            responseContent = responseContent.OrderBy(x => x.TemperatureC).ToArray();
            responseContent.Is(
                new[]
                {
                    new WeatherForecastResponse { City = "Tokyo", Summary = "Cold", Date = new DateTime(2020, 1, 1), TemperatureC = 0 },
                    new WeatherForecastResponse { City = "Tokyo", Summary = "Hot", Date = new DateTime(2020, 8, 6), TemperatureC = 35 },
                },
                (x, y) => x.City == y.City &&
                    x.Date == y.Date &&
                    x.Summary == y.Summary &&
                    x.TemperatureC == y.TemperatureC &&
                    x.TemperatureF == y.TemperatureF);
        }
    }
}
