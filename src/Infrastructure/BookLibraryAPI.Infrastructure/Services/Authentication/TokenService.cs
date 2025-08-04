using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using BookLibraryAPI.Application.Common.Services.Authentication;
using BookLibraryAPI.Core.Domain.Common;
using BookLibraryAPI.Core.Domain.Users;
using BookLibraryAPI.Infrastructure.Settings;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using JwtRegisteredClaimNames = Microsoft.IdentityModel.JsonWebTokens.JwtRegisteredClaimNames;

namespace BookLibraryAPI.Infrastructure.Services.Authentication;

public class TokenService(IOptions<JwtOptions> jwtOptions, ILogger<TokenService> logger) : ITokenService
{
    private readonly JwtOptions _jwtOptions = jwtOptions.Value;

    public Result<string> GenerateToken(User user)
    {
        try
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtOptions.Key));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.Role, user.Role.ToString()),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Iat, DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString(),
                    ClaimValueTypes.Integer64)
            };

            var token = new JwtSecurityToken(
                issuer: _jwtOptions.Issuer,
                audience: _jwtOptions.Audience,
                claims: claims,
                expires: DateTime.UtcNow.AddHours(1),
                signingCredentials: credentials
            );

            var tokenString = new JwtSecurityTokenHandler().WriteToken(token);
            
            logger.LogDebug("Token generated successfully for user: {Username}", user.Username);
            return Result<string>.Success(tokenString);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error generating token for user: {Username}", user.Username);
            throw;
        }
    }

    public Result<bool> ValidateToken(string token)
    {
        try
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(_jwtOptions.Key);
            
            tokenHandler.ValidateToken(token, new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = true,
                ValidIssuer = _jwtOptions.Issuer,
                ValidateAudience = true,
                ValidAudience = _jwtOptions.Audience,
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero
            }, out SecurityToken validatedToken);

            return Result<bool>.Success(true);
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "Token validation failed");
            return Result<bool>.Success(false);
        }
    }
}