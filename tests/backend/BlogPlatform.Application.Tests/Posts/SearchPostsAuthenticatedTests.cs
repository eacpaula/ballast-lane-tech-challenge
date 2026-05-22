using BlogPlatform.Application.Posts;
using BlogPlatform.Domain.Posts;

namespace BlogPlatform.Application.Tests.Posts;

public sealed class SearchPostsAuthenticatedTests
{
    [Fact]
    public async Task HandleAsync_ForAuthenticatedSearch_IncludesOwnedPrivateMatches()
    {
        var searchResults = new[]
        {
            BlogPost.Rehydrate(1, 2, 3, "Public blueprint", "Public summary", "Public content", isPublic: true, isAvailable: true),
            BlogPost.Rehydrate(2, 2, 3, "Private blueprint", "Owned summary", "Owned content", isPublic: false, isAvailable: true),
        };

        var repository = new SearchPostRepositoryStub(searchPosts: searchResults);
        var handler = new ListPublicPostsHandler(repository);

        var result = await handler.HandleAsync("blueprint", 2);

        Assert.Equal(2, result.Count);
        Assert.Contains(result, post => post.PostId == 2);
        Assert.Equal(2, repository.RequestedUserId);
    }

    [Fact]
    public async Task HandleAsync_ForAuthenticatedSearch_ExcludesPrivatePostsOwnedByOtherUsers()
    {
        var searchResults = new[]
        {
            BlogPost.Rehydrate(1, 2, 3, "Owned blueprint", "Owned summary", "Owned content", isPublic: false, isAvailable: true),
            BlogPost.Rehydrate(2, 1, 3, "Other blueprint", "Other summary", "Other content", isPublic: false, isAvailable: true),
            BlogPost.Rehydrate(3, 1, 3, "Public blueprint", "Public summary", "Public content", isPublic: true, isAvailable: true),
        };

        var repository = new SearchPostRepositoryStub(searchPosts: searchResults);
        var handler = new ListPublicPostsHandler(repository);

        var result = await handler.HandleAsync("blueprint", 2);

        Assert.Equal(2, result.Count);
        Assert.DoesNotContain(result, post => post.PostId == 2);
        Assert.Contains(result, post => post.PostId == 1);
        Assert.Contains(result, post => post.PostId == 3);
    }
}
