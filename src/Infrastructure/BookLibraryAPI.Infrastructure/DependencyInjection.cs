using BookLibraryAPI.Core.Domain.Interfaces.Repositories;
using BookLibraryAPI.Infrastructure.Persistence;
using BookLibraryAPI.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace BookLibraryAPI.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("LibraryDbConnection");
        
        services.AddDbContext<LibraryDbContext>(options =>
            options.UseNpgsql(connectionString, npgsqlOptions =>
                npgsqlOptions.MigrationsAssembly(typeof(LibraryDbContext).Assembly.FullName)));
        
        services.AddScoped<IBookRepository, BookRepository>();

        return services;
    }
    
}