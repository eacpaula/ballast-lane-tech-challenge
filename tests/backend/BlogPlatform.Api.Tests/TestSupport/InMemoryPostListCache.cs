using BlogPlatform.Application.Abstractions;
using BlogPlatform.Application.Posts;

namespace BlogPlatform.Api.Tests.TestSupport;

internal sealed class InMemoryPostListCache : IPostListCache
{
    private readonly Dictionary<string, PaginatedPublicPostResult> _entries = [];

    public Task<PaginatedPublicPostResult?> GetAsync(string cacheKey, CancellationToken cancellationToken = default)
        => Task.FromResult(_entries.TryGetValue(cacheKey, out var value) ? value : null);

    public Task SetAsync(
        string cacheKey,
        PaginatedPublicPostResult value,
        TimeSpan timeToLive,
        CancellationToken cancellationToken = default)
    {
        _entries[cacheKey] = value;
        return Task.CompletedTask;
    }
}
