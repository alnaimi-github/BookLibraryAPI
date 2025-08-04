using BookLibraryAPI.Application.Common.Services.Authentication;
using BookLibraryAPI.Core.Domain.Interfaces.Repositories;
using BookLibraryAPI.Infrastructure.Persistence;
using BookLibraryAPI.Infrastructure.Repositories;
using BookLibraryAPI.Infrastructure.Repositories.Books;
using BookLibraryAPI.Infrastructure.Repositories.Users;
using BookLibraryAPI.Infrastructure.Services.Authentication;
using BookLibraryAPI.Infrastructure.Settings;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace BookLibraryAPI.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        AddAuthenticationService(services, configuration);
        
        var connectionString = configuration.GetConnectionString("LibraryDbConnection");
        
        services.AddDbContext<LibraryDbContext>(options =>
            options.UseNpgsql(connectionString, npgsqlOptions =>
                npgsqlOptions.MigrationsAssembly(typeof(LibraryDbContext).Assembly.FullName)));
        
        services.AddScoped<IBookRepository, BookRepository>();
        
        services.AddScoped<IUserRepository, UserRepository>();

       

        return services;
    }
    
    
    private static void AddAuthenticationService(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<JwtOptions>(configuration.GetSection("Jwt"));
        var jwtOptions = configuration.GetSection("Jwt").Get<JwtOptions>();
        services
            .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtOptions.Key)),
                    ValidateIssuer = true,
                    ValidIssuer = jwtOptions.Issuer,
                    ValidateAudience = true,
                    ValidAudience = jwtOptions.Audience,
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero
                };
            });
        
        services.AddScoped<IAuthenticationService, AuthenticationService>();
        services.AddScoped<ITokenService, TokenService>();
    }
    
}