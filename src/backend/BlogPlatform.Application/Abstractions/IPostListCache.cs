using BlogPlatform.Application.Posts;

namespace BlogPlatform.Application.Abstractions;

public interface IPostListCache
{
    Task<PaginatedPublicPostResult?> GetAsync(string cacheKey, CancellationToken cancellationToken = default);

    Task SetAsync(
        string cacheKey,
        PaginatedPublicPostResult value,
        TimeSpan timeToLive,
        CancellationToken cancellationToken = default);
}
