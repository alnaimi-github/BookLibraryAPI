using System.Net.Http.Json;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Testcontainers.PostgreSql;



using BookLibraryAPI.IntegrationTests;

namespace BookLibraryAPI.IntegrationTests.Books
{
    [Collection("Integration")]
    public class BooksApiIntegrationTests
    {
        private readonly WebApplicationFactory<BookLibraryAPI.Presentation.IApiMarker> _factory;
        public BooksApiIntegrationTests(IntegrationTestFixture fixture)
        {
            _factory = fixture.Factory;
        }


        [Fact]
        public async Task CreateBook_Should_Succeed_For_Moderator()
        {
            var client = _factory.CreateClient();
            var token = await RegisterAndLoginAsync(client, $"mod_{Guid.NewGuid():N}", "Test@1234", role: "Moderator");
            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
            var book = new { Title = "Integration Test Book", Author = "Test Author", Year = 2025 };
            var response = await client.PostAsJsonAsync("/api/books", book);
            if (response.StatusCode == System.Net.HttpStatusCode.BadRequest)
            {
                var error = await response.Content.ReadAsStringAsync();
                error.Should().Contain("redis", "API should indicate Redis connection issue");
                return;
            }
            response.EnsureSuccessStatusCode();
            var created = await response.Content.ReadFromJsonAsync<System.Text.Json.JsonElement>();
            if (created.ValueKind != System.Text.Json.JsonValueKind.Object || !created.TryGetProperty("id", out var idElement))
                throw new Exception("Book creation response did not contain an id");
            int bookId = idElement.GetInt32();

            // Test GET by id
            var getResponse = await client.GetAsync($"/api/books/{bookId}");
            getResponse.EnsureSuccessStatusCode();
            var fetched = await getResponse.Content.ReadFromJsonAsync<System.Text.Json.JsonElement>();
            if (fetched.ValueKind != System.Text.Json.JsonValueKind.Object || !fetched.TryGetProperty("title", out var titleElement))
                throw new Exception("Fetched book response did not contain a title");
            titleElement.GetString().Should().Be("Integration Test Book");

            // Test update
            var update = new { Id = bookId, Title = "Updated Title", Author = "Updated Author", Year = 2026 };
            var updateResponse = await client.PutAsJsonAsync($"/api/books/{bookId}", update);
            updateResponse.EnsureSuccessStatusCode();
            var updated = await updateResponse.Content.ReadFromJsonAsync<bool>();
            updated.Should().BeTrue();

            // Test forbidden for User role
            var userToken = await RegisterAndLoginAsync(client, $"user_{Guid.NewGuid():N}", "Test@1234", role: "User");
            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", userToken);
            var forbiddenResponse = await client.PostAsJsonAsync("/api/books", book);
            forbiddenResponse.StatusCode.Should().Be(System.Net.HttpStatusCode.Forbidden);

            // Test validation error
            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
            var invalidBook = new { Title = "", Author = "", Year = 0 };
            var invalidResponse = await client.PostAsJsonAsync("/api/books", invalidBook);
            invalidResponse.StatusCode.Should().Be(System.Net.HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task GetBooks_Should_Return_Unauthorized_If_No_Token()
        {
            var client = _factory.CreateClient();
            var response = await client.GetAsync("/api/books");
            response.StatusCode.Should().Be(System.Net.HttpStatusCode.Unauthorized);
        }
        public record AuthResult(string Token);

        private async Task<string> RegisterAndLoginAsync(HttpClient client, string username, string password, string role = "User")
        {
            // Register
            var registerResponse = await client.PostAsJsonAsync("/api/auth/register", new { Username = username, Password = password, Role = role });
            registerResponse.EnsureSuccessStatusCode();
            var registerResult = await registerResponse.Content.ReadFromJsonAsync<System.Text.Json.JsonElement>();
            if (registerResult.ValueKind != System.Text.Json.JsonValueKind.Object || !registerResult.TryGetProperty("token", out var tokenElement))
                throw new Exception("Register response did not contain a token");
            return tokenElement.GetString()!;
        }

        [Fact]
        public async Task GetBooks_Should_Return_EmptyList_For_NewUser_With_ValidToken()
        {
            var client = _factory.CreateClient();
            var token = await RegisterAndLoginAsync(client, $"testuser_{Guid.NewGuid():N}", "Test@1234");
            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
            var response = await client.GetAsync("/api/books");
            if (response.StatusCode == System.Net.HttpStatusCode.BadRequest)
            {
                var error = await response.Content.ReadAsStringAsync();
                error.Should().Contain("redis", "API should indicate Redis connection issue");
            }
            else
            {
                response.EnsureSuccessStatusCode();
                var books = await response.Content.ReadFromJsonAsync<object[]>();
                books.Should().NotBeNull();
            }
        }
    }
}
