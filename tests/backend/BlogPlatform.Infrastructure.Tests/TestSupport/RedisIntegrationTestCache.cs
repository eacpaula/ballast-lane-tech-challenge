using BlogPlatform.Infrastructure.Configuration;
using StackExchange.Redis;

namespace BlogPlatform.Infrastructure.Tests;

public sealed class RedisIntegrationTestCache : IAsyncLifetime
{
    private readonly ConnectionMultiplexer _connection;

    public RedisIntegrationTestCache()
    {
        var settings = CreateSettings();
        _connection = ConnectionMultiplexer.Connect(settings.BuildConnectionString());
    }

    public Task InitializeAsync() => Task.CompletedTask;

    public async Task DisposeAsync() => await _connection.CloseAsync();

    public RedisCacheSettings CreateSettings()
    {
        static string GetValue(string name, string fallback)
        {
            var value = Environment.GetEnvironmentVariable(name);
            return !string.IsNullOrWhiteSpace(value) ? value.Trim() : fallback;
        }

        var portValue = GetValue("BLOG_PLATFORM_REDIS_PORT", "6379");

        return new RedisCacheSettings(
            host: GetValue("BLOG_PLATFORM_REDIS_HOST", "localhost"),
            port: int.TryParse(portValue, out var port) ? port : 6379);
    }

    public async Task ResetAsync()
    {
        var database = _connection.GetDatabase();
        await database.ExecuteAsync("FLUSHDB");
    }

    public async Task VerifyConnectivityAsync()
    {
        var database = _connection.GetDatabase();
        var result = await database.ExecuteAsync("PING");

        if (!string.Equals(result.ToString(), "PONG", StringComparison.OrdinalIgnoreCase))
        {
            throw new InvalidOperationException("The Redis cache is not available. Start docker compose redis before running Redis-backed Infrastructure tests.");
        }
    }
}
