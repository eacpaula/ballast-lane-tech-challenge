using BlogPlatform.Application.Posts;
using BlogPlatform.Domain.Posts;

namespace BlogPlatform.Application.Tests.Posts;

public sealed class SearchPostsAnonymousTests
{
    [Fact]
    public async Task HandleAsync_WithEmptySearch_UsesDefaultPublicListing()
    {
        var publicPosts = new[]
        {
            BlogPost.Rehydrate(1, 2, 3, "Public title", "Visible summary", "Visible content", isPublic: true, isAvailable: true),
        };

        var repository = new SearchPostRepositoryStub(publicPosts: publicPosts, searchPosts: []);
        var handler = new ListPublicPostsHandler(repository);

        var result = await handler.HandleAsync("   ", null);

        Assert.Single(result);
        Assert.True(repository.ListPublicReadWasCalled);
        Assert.False(repository.SearchPublicReadWasCalled);
    }

    [Fact]
    public async Task HandleAsync_ForAnonymousSearch_ReturnsOnlyPublicAvailableMatches()
    {
        var searchResults = new[]
        {
            BlogPost.Rehydrate(1, 2, 3, "Visible architecture", "Public summary", "Public content", isPublic: true, isAvailable: true),
            BlogPost.Rehydrate(2, 2, 3, "Private architecture", "Private summary", "Private content", isPublic: false, isAvailable: true),
            BlogPost.Rehydrate(3, 2, 3, "Unavailable architecture", "Unavailable summary", "Unavailable content", isPublic: true, isAvailable: false),
        };

        var repository = new SearchPostRepositoryStub(searchPosts: searchResults);
        var handler = new ListPublicPostsHandler(repository);

        var result = await handler.HandleAsync("architecture", null);

        Assert.Single(result);
        Assert.Equal(1, result[0].PostId);
        Assert.True(repository.SearchPublicReadWasCalled);
        Assert.Equal("architecture", repository.RequestedQuery);
        Assert.Null(repository.RequestedUserId);
    }

    [Fact]
    public async Task HandleAsync_WhenNoSearchMatches_ReturnsEmptyResult()
    {
        var repository = new SearchPostRepositoryStub(searchPosts: []);
        var handler = new ListPublicPostsHandler(repository);

        var result = await handler.HandleAsync("does-not-exist", null);

        Assert.Empty(result);
    }
}
