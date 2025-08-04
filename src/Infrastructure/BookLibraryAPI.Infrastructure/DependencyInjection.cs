using BookLibraryAPI.Application.Common.Services.Authentication;
using BookLibraryAPI.Core.Domain.Interfaces.Repositories;
using BookLibraryAPI.Infrastructure.Persistence;
using BookLibraryAPI.Infrastructure.Repositories;
using BookLibraryAPI.Infrastructure.Repositories.Books;
using BookLibraryAPI.Infrastructure.Repositories.Users;
using BookLibraryAPI.Infrastructure.Services.Authentication;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace BookLibraryAPI.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        AddAuthenticationService(services);
        
        var connectionString = configuration.GetConnectionString("LibraryDbConnection");
        
        services.AddDbContext<LibraryDbContext>(options =>
            options.UseNpgsql(connectionString, npgsqlOptions =>
                npgsqlOptions.MigrationsAssembly(typeof(LibraryDbContext).Assembly.FullName)));
        
        services.AddScoped<IBookRepository, BookRepository>();
        
        services.AddScoped<IUserRepository, UserRepository>();

       

        return services;
    }
    
    
    private static void AddAuthenticationService(this IServiceCollection services)
    {
        services.AddScoped<IAuthenticationService, AuthenticationService>();
        services.AddScoped<ITokenService, TokenService>();
    }
    
}