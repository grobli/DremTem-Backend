using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Threading.Tasks;
using ClientApiGateway.Api;
using Microsoft.AspNetCore.Mvc.Testing;
using Shared.Proto;
using Xunit;

namespace ClientApiGateway.IntegrationTests
{
    public class IntegrationTest : IDisposable
    {
        protected readonly HttpClient TestClient;
        protected UserCredentials TestUser;
        protected UserCredentials AdminCredentials = new("admin", "Password@123");
        protected readonly string BaseUrl = "https://localhost:8081";

        protected IntegrationTest()
        {
            //var appFactory = new WebApplicationFactory<Startup>();
            TestClient = new HttpClient();
        }

        protected async Task AuthenticateAsync()
        {
            await CreateTestUser();
            TestClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", await GetJwtAsync(TestUser));
        }

        protected record UserCredentials(string Email, string Password);

        private async Task CreateTestUser()
        {
            TestUser = new UserCredentials($"{Guid.NewGuid()}@integration.test", "Password@123");
            var request = new UserSignUpRequest
                { Email = TestUser.Email, Password = TestUser.Password };
            await TestClient.PostAsJsonAsync($"{BaseUrl}/api/v1/auth/signup", request);
        }
        
        private async Task<string> GetJwtAsync(UserCredentials credentials)
        {
            var (email, password) = credentials;
            var request = new UserLoginRequest
                { Email = email, Password = password };
            var response = await TestClient.PostAsJsonAsync($"{BaseUrl}/api/v1/auth/login", request);
            var loginResponse = await response.Content.ReadAsAsync<UserLoginResponse>();
            return loginResponse.JwtToken;
        }

        private async Task DeleteTestUser()
        {
            TestClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", await GetJwtAsync(AdminCredentials));

            var response = await TestClient.GetAsync($"{BaseUrl}/api/v1/Users/{TestUser.Email}");
            var user = await response.Content.ReadAsAsync<UserDto>();

            await TestClient.DeleteAsync($"{BaseUrl}/api/v1/Users/{user.Id}");
        }

        public  void Dispose()
        {
            Task.WaitAll(DeleteTestUser());
            TestClient?.Dispose();
        }
    }
}