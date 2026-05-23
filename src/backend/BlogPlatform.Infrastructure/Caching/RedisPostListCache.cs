using System.Text.Json;
using BlogPlatform.Application.Abstractions;
using BlogPlatform.Application.Posts;

namespace BlogPlatform.Infrastructure.Caching;

public sealed class RedisPostListCache : IPostListCache
{
    private static readonly JsonSerializerOptions SerializerOptions = new(JsonSerializerDefaults.Web);
    private readonly RedisConnectionFactory _connectionFactory;

    public RedisPostListCache(RedisConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory ?? throw new ArgumentNullException(nameof(connectionFactory));
    }

    public async Task<PaginatedPublicPostResult?> GetAsync(string cacheKey, CancellationToken cancellationToken = default)
    {
        var database = _connectionFactory.GetDatabase();
        var payload = await database.StringGetAsync(cacheKey);

        if (!payload.HasValue)
        {
            return null;
        }

        return JsonSerializer.Deserialize<PaginatedPublicPostResult>(payload.ToString(), SerializerOptions);
    }

    public Task SetAsync(
        string cacheKey,
        PaginatedPublicPostResult value,
        TimeSpan timeToLive,
        CancellationToken cancellationToken = default)
    {
        var database = _connectionFactory.GetDatabase();
        var payload = JsonSerializer.Serialize(value, SerializerOptions);
        return database.StringSetAsync(cacheKey, payload, timeToLive);
    }
}
