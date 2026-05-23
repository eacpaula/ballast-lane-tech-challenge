using Microsoft.Extensions.Configuration;

namespace BlogPlatform.Infrastructure.Configuration;

public sealed class RedisCacheSettings
{
    public RedisCacheSettings(string host, int port)
    {
        Host = RequireValue(host, nameof(host));
        Port = port > 0 ? port : throw new ArgumentOutOfRangeException(nameof(port));
    }

    public string Host { get; }

    public int Port { get; }

    public string BuildConnectionString() => $"{Host}:{Port}";

    public static RedisCacheSettings FromConfiguration(IConfiguration configuration)
    {
        ArgumentNullException.ThrowIfNull(configuration);

        string GetValue(string sectionKey, string envKey, string fallback)
        {
            var value = Environment.GetEnvironmentVariable(envKey);

            if (!string.IsNullOrWhiteSpace(value))
            {
                return value.Trim();
            }

            value = configuration[$"Redis:{sectionKey}"];

            return !string.IsNullOrWhiteSpace(value)
                ? value.Trim()
                : fallback;
        }

        var portValue = GetValue("Port", "BLOG_PLATFORM_REDIS_PORT", "6379");

        return new RedisCacheSettings(
            host: GetValue("Host", "BLOG_PLATFORM_REDIS_HOST", "localhost"),
            port: int.TryParse(portValue, out var port) ? port : 6379);
    }

    private static string RequireValue(string value, string paramName)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new ArgumentException($"{paramName} is required.", paramName);
        }

        return value.Trim();
    }
}
