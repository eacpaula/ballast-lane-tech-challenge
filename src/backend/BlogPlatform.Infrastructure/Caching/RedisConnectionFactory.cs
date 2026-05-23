using BlogPlatform.Infrastructure.Configuration;
using StackExchange.Redis;

namespace BlogPlatform.Infrastructure.Caching;

public sealed class RedisConnectionFactory : IDisposable
{
    private readonly Lazy<ConnectionMultiplexer> _lazyConnection;

    public RedisConnectionFactory(RedisCacheSettings settings)
    {
        ArgumentNullException.ThrowIfNull(settings);
        _lazyConnection = new Lazy<ConnectionMultiplexer>(() => ConnectionMultiplexer.Connect(settings.BuildConnectionString()));
    }

    public IDatabase GetDatabase() => _lazyConnection.Value.GetDatabase();

    public void Dispose()
    {
        if (_lazyConnection.IsValueCreated)
        {
            _lazyConnection.Value.Dispose();
        }
    }
}
