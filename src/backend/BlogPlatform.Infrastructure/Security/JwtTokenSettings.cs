namespace BlogPlatform.Infrastructure.Security;

public sealed class JwtTokenSettings
{
    public JwtTokenSettings(string issuer, string audience, string signingKey, int expirationMinutes)
    {
        Issuer = RequireValue(issuer, nameof(issuer));
        Audience = RequireValue(audience, nameof(audience));
        SigningKey = RequireValue(signingKey, nameof(signingKey));
        ExpirationMinutes = expirationMinutes > 0 ? expirationMinutes : throw new ArgumentOutOfRangeException(nameof(expirationMinutes));
    }

    public string Issuer { get; }
    public string Audience { get; }
    public string SigningKey { get; }
    public int ExpirationMinutes { get; }

    private static string RequireValue(string value, string paramName)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new ArgumentException($"{paramName} is required.", paramName);
        }

        return value.Trim();
    }
}
