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
using BookLibraryAPI.Core.Domain.Users.Enums;

namespace BookLibraryAPI.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, 
        IConfiguration configuration)
    {
        AddAuthentication(services, configuration);

        AddAuthorization(services);
        
        AddPersistence(services,configuration);
        
        AddRepositories(services);

        return services;
    }
    
    
    private static void AddAuthentication(IServiceCollection services, IConfiguration configuration)
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
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtOptions!.Key)),
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
    
    private static void AddRepositories(IServiceCollection services)
    {
        services.AddScoped<IBookRepository, BookRepository>();
        services.AddScoped<IUserRepository, UserRepository>();
    }
    
    private static void AddPersistence( IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("LibraryDbConnection");
        
        services.AddDbContext<LibraryDbContext>(options =>
            options.UseNpgsql(connectionString, npgsqlOptions =>
                npgsqlOptions.MigrationsAssembly(typeof(LibraryDbContext).Assembly.FullName)));
    }
    private static void AddAuthorization(IServiceCollection services)
    {
        services.AddAuthorization(options =>
        {
            options.AddPolicy("ModeratorOrAdmin", policy =>
                policy.RequireRole(nameof(UserRole.Moderator), nameof(UserRole.Admin)));
        });
    }

}