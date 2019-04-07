//using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using StoreApi;
using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;

namespace Test
{
    public class ApiAuthTests : IClassFixture<CustomWebApplicationFactory<Startup>>
    {

        /// <summary>
        /// Test Init
        /// </summary>
        public ApiAuthTests(CustomWebApplicationFactory<Startup> factory)
        {
            client = factory.CreateClient();
        }


        [Fact]
        public async Task GetAllProductsTestAsync()
        {
            var request = new HttpRequestMessage(new HttpMethod("get"), "/api/products/");

            var response = await client.SendAsync(request);

            response.EnsureSuccessStatusCode();

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var content = await response.Content.ReadAsStringAsync();


        }

        private readonly HttpClient client;

        private TestServer server { get; set; }
    }
}
