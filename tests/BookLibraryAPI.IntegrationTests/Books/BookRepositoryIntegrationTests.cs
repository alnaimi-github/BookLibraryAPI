using System.Threading.Tasks;
using BookLibraryAPI.Core.Domain.Books;
using BookLibraryAPI.Infrastructure.Persistence;
using BookLibraryAPI.Infrastructure.Repositories.Books;
using FluentAssertions;
using Testcontainers.PostgreSql;
using Xunit;
using Microsoft.EntityFrameworkCore;

namespace BookLibraryAPI.IntegrationTests.Books;

public class BookRepositoryIntegrationTests : IAsyncLifetime
{
    private readonly PostgreSqlContainer _pgContainer = new PostgreSqlBuilder()
        .WithDatabase("testdb")
        .WithUsername("postgres")
        .WithPassword("postgres")
        .Build();

    private LibraryDbContext _dbContext = null!;
    private BookRepository _repository = null!;

    public async Task InitializeAsync()
    {
        await _pgContainer.StartAsync();
        var options = new DbContextOptionsBuilder<LibraryDbContext>()
            .UseNpgsql(_pgContainer.GetConnectionString())
            .Options;
        _dbContext = new LibraryDbContext(options);
        await _dbContext.Database.EnsureCreatedAsync();
        _repository = new BookRepository(_dbContext);
    }

    public async Task DisposeAsync()
    {
        await _pgContainer.DisposeAsync();
    }

    [Fact]
    public async Task AddAsync_Should_Add_And_Retrieve_Book()
    {
        var book = Book.Create("Integration Test Book", "Test Author", 2025);
        var added = await _repository.AddAsync(book);
        var fetched = await _repository.GetByIdAsync(added.Id);
        fetched.Should().NotBeNull();
        fetched!.Title.Value.Should().Be("Integration Test Book");
    }
}
