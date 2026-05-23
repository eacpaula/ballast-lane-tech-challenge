using BlogPlatform.Application.Posts;
using BlogPlatform.Domain.Posts;

namespace BlogPlatform.Application.Tests.Posts;

public sealed class ListPublicPostsCacheTests
{
    [Fact]
    public async Task HandleAsync_UsesViewerSpecificCacheKeyForAnonymousRequests()
    {
        var cachedResult = new PaginatedPublicPostResult(
        [
            new PublicPostListItem(1, "Cached title", "Cached summary", []),
        ],
        page: 1,
        pageSize: 6,
        totalCount: 1);
        var repository = new PaginatedPostRepositoryStub();
        var cache = new PostListCacheStub();
        cache.Seed("posts:list|query:cached|page:1|size:6|viewer:anonymous", cachedResult);
        var handler = new ListPublicPostsHandler(repository, cache);

        var result = await handler.HandleAsync("cached", page: 1, pageSize: 6);

        Assert.Single(result.Items);
        Assert.Equal(0, repository.Calls);
        Assert.Equal("posts:list|query:cached|page:1|size:6|viewer:anonymous", cache.LastGetKey);
    }

    [Fact]
    public async Task HandleAsync_WhenCacheMisses_PopulatesCacheWithThirtySecondTtl()
    {
        var repository = new PaginatedPostRepositoryStub(
            new PaginatedBlogPostReadResult(
            [
                BlogPost.Rehydrate(1, 2, 3, "Public title", "Visible summary", "Visible content", isPublic: true, isAvailable: true),
            ],
            totalCount: 1));
        var cache = new PostListCacheStub();
        var handler = new ListPublicPostsHandler(repository, cache);

        await handler.HandleAsync("public", page: 1, pageSize: 6, requestingUserId: 2);

        Assert.Equal("posts:list|query:public|page:1|size:6|viewer:user:2", cache.LastSetKey);
        Assert.Equal(TimeSpan.FromSeconds(30), cache.LastTtl);
    }
}
