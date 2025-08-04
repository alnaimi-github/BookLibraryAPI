namespace BookLibraryAPI.Infrastructure.Settings;

public class JwtOptions
{
    public required string Key { get; init; } = string.Empty;
    public required string Issuer { get; init; } = string.Empty;
    public required string Audience { get; init; } = string.Empty;
}

