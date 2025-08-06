using System.Net;
using System.Net.Http.Json;
using System.Threading.Tasks;
using BookLibraryAPI.Presentation;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Testcontainers.PostgreSql;
using Xunit;

namespace BookLibraryAPI.IntegrationTests.Books;

public class BooksApiIntegrationTests : IClassFixture<WebApplicationFactory<Program>>, IAsyncLifetime
{
    private readonly WebApplicationFactory<Program> _factory;
    private readonly PostgreSqlContainer _pgContainer = new PostgreSqlBuilder()
        .WithDatabase("testdb")
        .WithUsername("postgres")
        .WithPassword("postgres")
        .Build();

    public BooksApiIntegrationTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory.WithWebHostBuilder(builder =>
        {
          
        });
    }

    public async Task InitializeAsync()
    {
        await _pgContainer.StartAsync();

    }

    public async Task DisposeAsync()
    {
        await _pgContainer.DisposeAsync();
    }

    [Fact(Skip = "Requires DB config override for container connection string")]
    public async Task GetBooks_Should_Return_Unauthorized_If_No_Token()
    {
        var client = _factory.CreateClient();
        var response = await client.GetAsync("/api/books");
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }
}
