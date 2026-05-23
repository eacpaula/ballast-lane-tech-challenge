using BlogPlatform.Application.Abstractions;
using BlogPlatform.Application.Posts;

namespace BlogPlatform.Application.Tests.Posts;

internal sealed class PostListCacheStub : IPostListCache
{
    private readonly Dictionary<string, PaginatedPublicPostResult> _values = [];

    public string? LastGetKey { get; private set; }

    public string? LastSetKey { get; private set; }

    public TimeSpan? LastTtl { get; private set; }

    public Task<PaginatedPublicPostResult?> GetAsync(string cacheKey, CancellationToken cancellationToken = default)
    {
        LastGetKey = cacheKey;
        return Task.FromResult(_values.TryGetValue(cacheKey, out var value) ? value : null);
    }

    public Task SetAsync(
        string cacheKey,
        PaginatedPublicPostResult value,
        TimeSpan timeToLive,
        CancellationToken cancellationToken = default)
    {
        LastSetKey = cacheKey;
        LastTtl = timeToLive;
        _values[cacheKey] = value;
        return Task.CompletedTask;
    }

    public void Seed(string cacheKey, PaginatedPublicPostResult value) => _values[cacheKey] = value;
}
