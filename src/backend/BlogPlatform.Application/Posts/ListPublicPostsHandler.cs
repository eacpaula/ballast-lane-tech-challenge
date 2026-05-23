using BlogPlatform.Application.Abstractions;

namespace BlogPlatform.Application.Posts;

public sealed class ListPublicPostsHandler
{
    private static readonly TimeSpan CacheTtl = TimeSpan.FromSeconds(30);
    private readonly IPostRepository _postRepository;
    private readonly IPostListCache _postListCache;

    public ListPublicPostsHandler(IPostRepository postRepository, IPostListCache? postListCache = null)
    {
        _postRepository = postRepository ?? throw new ArgumentNullException(nameof(postRepository));
        _postListCache = postListCache ?? NullPostListCache.Instance;
    }

    public async Task<IReadOnlyList<PublicPostListItem>> HandleAsync(
        string? query = null,
        int? requestingUserId = null,
        CancellationToken cancellationToken = default)
    {
        var result = await HandleAsync(
            query,
            PostListPageRequest.DefaultPage,
            PostListPageRequest.DefaultPageSize,
            requestingUserId,
            cancellationToken);

        return result.Items;
    }

    public async Task<PaginatedPublicPostResult> HandleAsync(
        string? query,
        int page,
        int pageSize,
        int? requestingUserId = null,
        CancellationToken cancellationToken = default)
    {
        var request = PostListPageRequest.Create(query, page, pageSize, requestingUserId);
        var cacheKey = PostListCacheKeyFactory.Create(request);
        var cached = await _postListCache.GetAsync(cacheKey, cancellationToken);

        if (cached is not null)
        {
            return cached;
        }

        var posts = await _postRepository.ListPublicReadPageAsync(request, cancellationToken);
        var visiblePosts = posts.Items
            .Where(post => IsVisibleToRequester(post, request))
            .Select(PublicPostListItem.From)
            .ToArray();
        var result = new PaginatedPublicPostResult(visiblePosts, request.Page, request.PageSize, posts.TotalCount);

        await _postListCache.SetAsync(cacheKey, result, CacheTtl, cancellationToken);

        return result;
    }

    private static bool IsVisibleToRequester(BlogPlatform.Domain.Posts.BlogPost post, PostListPageRequest request)
    {
        if (request.Query is null)
        {
            return post.IsPubliclyReadable;
        }

        return post.IsPubliclyReadable
            || (request.RequestingUserId.HasValue && post.IsAvailable && post.AuthorUserId == request.RequestingUserId.Value);
    }

    private sealed class NullPostListCache : IPostListCache
    {
        public static IPostListCache Instance { get; } = new NullPostListCache();

        public Task<PaginatedPublicPostResult?> GetAsync(string cacheKey, CancellationToken cancellationToken = default)
            => Task.FromResult<PaginatedPublicPostResult?>(null);

        public Task SetAsync(
            string cacheKey,
            PaginatedPublicPostResult value,
            TimeSpan timeToLive,
            CancellationToken cancellationToken = default)
            => Task.CompletedTask;
    }
}
