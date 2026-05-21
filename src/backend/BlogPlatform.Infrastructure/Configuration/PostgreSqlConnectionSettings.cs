using Microsoft.Extensions.Configuration;
using Npgsql;

namespace BlogPlatform.Infrastructure.Configuration;

public sealed class PostgreSqlConnectionSettings
{
    public PostgreSqlConnectionSettings(
        string host,
        int port,
        string database,
        string username,
        string password)
    {
        Host = RequireValue(host, nameof(host));
        Port = port > 0 ? port : throw new ArgumentOutOfRangeException(nameof(port));
        Database = RequireValue(database, nameof(database));
        Username = RequireValue(username, nameof(username));
        Password = RequireValue(password, nameof(password));
    }

    public string Host { get; }

    public int Port { get; }

    public string Database { get; }

    public string Username { get; }

    public string Password { get; }

    public string BuildConnectionString()
    {
        var builder = new NpgsqlConnectionStringBuilder
        {
            Host = Host,
            Port = Port,
            Database = Database,
            Username = Username,
            Password = Password,
            Pooling = false,
        };

        return builder.ConnectionString;
    }

    public static PostgreSqlConnectionSettings FromConfiguration(IConfiguration configuration)
    {
        ArgumentNullException.ThrowIfNull(configuration);

        string GetValue(string sectionKey, string envKey, string fallback)
        {
            var value = Environment.GetEnvironmentVariable(envKey);

            if (!string.IsNullOrWhiteSpace(value))
            {
                return value.Trim();
            }

            value = configuration[$"Database:{sectionKey}"];

            return !string.IsNullOrWhiteSpace(value)
                ? value.Trim()
                : fallback;
        }

        var portValue = GetValue("Port", "BLOG_PLATFORM_DB_PORT", "5432");

        return new PostgreSqlConnectionSettings(
            host: GetValue("Host", "BLOG_PLATFORM_DB_HOST", "localhost"),
            port: int.TryParse(portValue, out var port) ? port : 5432,
            database: GetValue("Name", "BLOG_PLATFORM_DB_NAME", "blogplatform"),
            username: GetValue("User", "BLOG_PLATFORM_DB_USER", "blogplatform"),
            password: GetValue("Password", "BLOG_PLATFORM_DB_PASSWORD", "blogplatform"));
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
