using IdentityModel.Client;
using IdentityServer4.Models;
using Newtonsoft.Json.Linq;
using StoreApi.Identity;
using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;

namespace XUnitIntegrationTest
{
    public class UnitTest1
    {
        [Fact]
        public async Task ResourceOwnerIntegrationTest()
        {
            await TestResourceOwnerIntegration();
        }


        /// <summary>
        /// Resource owner integration test for secured web api
        /// </summary>
        private async Task TestResourceOwnerIntegration()
        {
            // discover endpoints from metadata
            var client = new HttpClient();

            var disco = await client.GetDiscoveryDocumentAsync(Config.BaseUrl);
            if (disco.IsError)
            {
                Console.WriteLine(disco.Error);
                return;
            }

            var apiclient = Config.ResourceOwnerClient;

            if (client == null) return;

            // request token
            var tokenResponse = await client.RequestPasswordTokenAsync(new PasswordTokenRequest
            {
                Address = disco.TokenEndpoint,
                ClientId = apiclient.ClientId,
                ClientSecret = "secret",

                UserName = "admin@yopmail.com",
                Password = "1234567",
                Scope = "api1"
            });

            if (tokenResponse.IsError)
            {
                Console.WriteLine(tokenResponse.Error);
                return;
            }

            Console.WriteLine(tokenResponse.Json);
            Console.WriteLine("\n\n");

            // call api
            var apiClient = new HttpClient();
            apiClient.SetBearerToken(tokenResponse.AccessToken);

            apiClient.DefaultRequestHeaders.Add("bearer", tokenResponse.AccessToken);


            var response = await apiClient.GetAsync($"{Config.BaseUrl}api/identity");

            Assert.True(response.IsSuccessStatusCode);
            if (!response.IsSuccessStatusCode)
            {
                Console.WriteLine(response.StatusCode);
            }
            else
            {
                var content = response.Content.ReadAsStringAsync().Result;
                Console.WriteLine(content);
            }

            var values = await apiClient.GetAsync($"{Config.BaseUrl}api/identity/Adminclaims");

            Assert.True(values.IsSuccessStatusCode);
            if (!values.IsSuccessStatusCode)
            {
                Console.WriteLine(response.StatusCode);
            }
            else
            {
                var content = values.Content.ReadAsStringAsync().Result;
                Console.WriteLine(content);
            }
        }
    }
}
