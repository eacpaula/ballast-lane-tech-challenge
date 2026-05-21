using BlogPlatform.Infrastructure.Security;
using Microsoft.Extensions.Configuration;

namespace BlogPlatform.Api.Configuration;

public sealed class JwtAuthenticationSettings
{
    public JwtAuthenticationSettings(string issuer, string audience, string signingKey, int expirationMinutes)
    {
        Issuer = issuer;
        Audience = audience;
        SigningKey = signingKey;
        ExpirationMinutes = expirationMinutes;
    }

    public string Issuer { get; }
    public string Audience { get; }
    public string SigningKey { get; }
    public int ExpirationMinutes { get; }

    public JwtTokenSettings ToInfrastructureSettings()
        => new(Issuer, Audience, SigningKey, ExpirationMinutes);

    public static JwtAuthenticationSettings FromConfiguration(IConfiguration configuration)
    {
        ArgumentNullException.ThrowIfNull(configuration);

        string GetValue(string sectionKey, string envKey, string fallback)
        {
            var value = configuration[$"Jwt:{sectionKey}"];

            if (!string.IsNullOrWhiteSpace(value))
            {
                return value.Trim();
            }

            value = Environment.GetEnvironmentVariable(envKey);
            return !string.IsNullOrWhiteSpace(value) ? value.Trim() : fallback;
        }

        var expirationValue = GetValue("ExpirationMinutes", "BLOG_PLATFORM_JWT_EXPIRATION_MINUTES", "60");

        return new JwtAuthenticationSettings(
            GetValue("Issuer", "BLOG_PLATFORM_JWT_ISSUER", "blogplatform-api"),
            GetValue("Audience", "BLOG_PLATFORM_JWT_AUDIENCE", "blogplatform-clients"),
            GetValue("SigningKey", "BLOG_PLATFORM_JWT_SIGNING_KEY", "super-secret-demo-signing-key-change-me"),
            int.TryParse(expirationValue, out var minutes) ? minutes : 60);
    }
}
