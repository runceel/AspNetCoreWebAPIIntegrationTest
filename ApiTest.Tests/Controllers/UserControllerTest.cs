using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace ApiTest.Tests.Controllers
{
    public class UserControllerTest : IClassFixture<CustomWebApplicationFactory<Startup>>
    {
        private readonly CustomWebApplicationFactory<Startup> _factory;

        public UserControllerTest(CustomWebApplicationFactory<Startup> factory)
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
            var res = await client.GetAsync("/User");
            res.StatusCode.Is(HttpStatusCode.Unauthorized);
        }

        [Fact]
        public async Task GetUserName()
        {
            var client = _factory.WithWebHostBuilder(b =>
            {
                b.ConfigureTestServices(services =>
                {
                    services.AddAuthentication("Test")
                        .AddScheme<AuthenticationSchemeOptions, TestAuthHandler>("Test", options => { });
                });
            }).CreateClient(new WebApplicationFactoryClientOptions { AllowAutoRedirect = false });
            var res = await client.GetAsync("/User");
            res.StatusCode.Is(HttpStatusCode.OK);
            var body = await res.Content.ReadAsStringAsync();
            body.Is("Test user");
        }
    }
}
