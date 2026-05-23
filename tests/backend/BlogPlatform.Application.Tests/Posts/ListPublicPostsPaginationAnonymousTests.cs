using BlogPlatform.Application.Posts;
using BlogPlatform.Domain.Posts;

namespace BlogPlatform.Application.Tests.Posts;

public sealed class ListPublicPostsPaginationAnonymousTests
{
    [Fact]
    public async Task HandleAsync_WithEmptySearch_UsesDefaultListingAndReturnsPageMetadata()
    {
        var repository = new PaginatedPostRepositoryStub(
            new PaginatedBlogPostReadResult(
            [
                BlogPost.Rehydrate(1, 2, 3, "Public title", "Visible summary", "Visible content", isPublic: true, isAvailable: true),
            ],
            totalCount: 1));
        var cache = new PostListCacheStub();
        var handler = new ListPublicPostsHandler(repository, cache);

        var result = await handler.HandleAsync("   ", page: 1, pageSize: 6);

        Assert.Single(result.Items);
        Assert.Equal(1, result.Page);
        Assert.Equal(6, result.PageSize);
        Assert.Equal(1, result.TotalCount);
        Assert.Equal(1, result.TotalPages);
        Assert.False(result.HasNextPage);
        Assert.Equal("posts:list|query:__all__|page:1|size:6|viewer:anonymous", cache.LastGetKey);
        Assert.NotNull(repository.RequestedPage);
        Assert.Null(repository.RequestedPage!.Query);
    }

    [Fact]
    public async Task HandleAsync_ForAnonymousSearch_ReturnsOnlyPublicAvailableMatches()
    {
        var repository = new PaginatedPostRepositoryStub(
            new PaginatedBlogPostReadResult(
            [
                BlogPost.Rehydrate(1, 2, 3, "Visible architecture", "Public summary", "Public content", isPublic: true, isAvailable: true),
                BlogPost.Rehydrate(2, 2, 3, "Private architecture", "Private summary", "Private content", isPublic: false, isAvailable: true),
                BlogPost.Rehydrate(3, 2, 3, "Unavailable architecture", "Unavailable summary", "Unavailable content", isPublic: true, isAvailable: false),
            ],
            totalCount: 3));
        var handler = new ListPublicPostsHandler(repository, new PostListCacheStub());

        var result = await handler.HandleAsync("architecture", page: 1, pageSize: 6);

        Assert.Single(result.Items);
        Assert.Equal(1, result.Items[0].PostId);
        Assert.Equal(3, result.TotalCount);
        Assert.NotNull(repository.RequestedPage);
        Assert.Equal("architecture", repository.RequestedPage!.Query);
    }

    [Fact]
    public async Task HandleAsync_WhenPageIsBeyondResultSet_ReturnsEmptyPage()
    {
        var repository = new PaginatedPostRepositoryStub(new PaginatedBlogPostReadResult([], totalCount: 2));
        var handler = new ListPublicPostsHandler(repository, new PostListCacheStub());

        var result = await handler.HandleAsync("does-not-exist", page: 3, pageSize: 2);

        Assert.Empty(result.Items);
        Assert.Equal(3, result.Page);
        Assert.Equal(2, result.PageSize);
        Assert.Equal(2, result.TotalCount);
        Assert.Equal(1, result.TotalPages);
        Assert.False(result.HasNextPage);
    }
}
