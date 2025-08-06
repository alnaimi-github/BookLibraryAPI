using System.Net.Http.Json;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Testcontainers.PostgreSql;
using Xunit;

using BookLibraryAPI.IntegrationTests;

namespace BookLibraryAPI.IntegrationTests.Users
{
    [Collection("Integration")]
    public class AuthenticationEndpointsIntegrationTests
    {
        private readonly WebApplicationFactory<BookLibraryAPI.Presentation.IApiMarker> _factory;
        public AuthenticationEndpointsIntegrationTests(IntegrationTestFixture fixture)
        {
            _factory = fixture.Factory;
        }

        [Fact]
        public async Task Register_Should_Succeed_For_NewUser()
        {
            var client = _factory.CreateClient();
            var register = new { Username = $"user_{Guid.NewGuid():N}", Password = "Test@1234", Role = "User" };
            var response = await client.PostAsJsonAsync("/api/auth/register", register);
            response.EnsureSuccessStatusCode();
            var result = await response.Content.ReadFromJsonAsync<System.Text.Json.JsonElement>();
            if (result.ValueKind != System.Text.Json.JsonValueKind.Object || !result.TryGetProperty("token", out var tokenElement))
                throw new Exception("Register response did not contain a token");
            var token = tokenElement.GetString();
            token.Should().NotBeNullOrEmpty();
        }

        [Fact]
        public async Task Login_Should_Succeed_For_RegisteredUser()
        {
            var client = _factory.CreateClient();
            var username = $"user_{Guid.NewGuid():N}";
            var password = "Test@1234";
            var register = new { Username = username, Password = password, Role = "User" };
            var regResponse = await client.PostAsJsonAsync("/api/auth/register", register);
            regResponse.EnsureSuccessStatusCode();

            var login = new { Username = username, Password = password };
            var loginResponse = await client.PostAsJsonAsync("/api/auth/login", login);
            loginResponse.EnsureSuccessStatusCode();
            var result = await loginResponse.Content.ReadFromJsonAsync<System.Text.Json.JsonElement>();
            if (result.ValueKind != System.Text.Json.JsonValueKind.Object || !result.TryGetProperty("token", out var tokenElement))
                throw new Exception("Login response did not contain a token");
            var token = tokenElement.GetString();
            token.Should().NotBeNullOrEmpty();
        }

        [Fact]
        public async Task Register_Should_Return_BadRequest_For_DuplicateUser()
        {
            var client = _factory.CreateClient();
            var username = $"user_{Guid.NewGuid():N}";
            var register = new { Username = username, Password = "Test@1234", Role = "User" };
            var response1 = await client.PostAsJsonAsync("/api/auth/register", register);
            response1.EnsureSuccessStatusCode();
            var response2 = await client.PostAsJsonAsync("/api/auth/register", register);
            response2.StatusCode.Should().Be(System.Net.HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task Login_Should_Return_BadRequest_For_InvalidPassword()
        {
            var client = _factory.CreateClient();
            var username = $"user_{Guid.NewGuid():N}";
            var password = "Test@1234";
            var register = new { Username = username, Password = password, Role = "User" };
            var regResponse = await client.PostAsJsonAsync("/api/auth/register", register);
            regResponse.EnsureSuccessStatusCode();

            var login = new { Username = username, Password = "WrongPassword" };
            var loginResponse = await client.PostAsJsonAsync("/api/auth/login", login);
            loginResponse.StatusCode.Should().Be(System.Net.HttpStatusCode.BadRequest);
        }
    }
}
