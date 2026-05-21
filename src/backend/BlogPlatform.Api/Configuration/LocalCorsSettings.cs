using Microsoft.Extensions.Configuration;

namespace BlogPlatform.Api.Configuration;

public sealed class LocalCorsSettings
{
    public LocalCorsSettings(IReadOnlyList<string> allowedOrigins)
    {
        AllowedOrigins = allowedOrigins
            .Where(origin => !string.IsNullOrWhiteSpace(origin))
            .Select(origin => origin.Trim())
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToArray();
    }

    public IReadOnlyList<string> AllowedOrigins { get; }

    public static LocalCorsSettings FromConfiguration(IConfiguration configuration)
    {
        ArgumentNullException.ThrowIfNull(configuration);

        var configuredOrigins = configuration
            .GetSection("Cors:AllowedOrigins")
            .Get<string[]>();

        if (configuredOrigins is { Length: > 0 })
        {
            return new LocalCorsSettings(configuredOrigins);
        }

        var environmentValue = Environment.GetEnvironmentVariable("BLOG_PLATFORM_FRONTEND_ORIGINS");

        if (!string.IsNullOrWhiteSpace(environmentValue))
        {
            return new LocalCorsSettings(environmentValue.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries));
        }

        return new LocalCorsSettings([
            "http://localhost:5173",
            "http://127.0.0.1:5173",
        ]);
    }
}
