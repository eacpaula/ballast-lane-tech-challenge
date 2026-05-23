using BlogPlatform.Application.Posts;
using BlogPlatform.Domain.Posts;

namespace BlogPlatform.Application.Tests.Posts;

public sealed class ListPublicPostsPaginationAuthenticatedTests
{
    [Fact]
    public async Task HandleAsync_ForAuthenticatedDefaultListing_RemainsPublicOnly()
    {
        var repository = new PaginatedPostRepositoryStub(
            new PaginatedBlogPostReadResult(
            [
                BlogPost.Rehydrate(1, 2, 3, "Public blueprint", "Public summary", "Public content", isPublic: true, isAvailable: true),
                BlogPost.Rehydrate(2, 2, 3, "Private blueprint", "Owned summary", "Owned content", isPublic: false, isAvailable: true),
            ],
            totalCount: 1));
        var handler = new ListPublicPostsHandler(repository, new PostListCacheStub());

        var result = await handler.HandleAsync(null, page: 1, pageSize: 6, requestingUserId: 2);

        Assert.Single(result.Items);
        Assert.DoesNotContain(result.Items, post => post.PostId == 2);
    }

    [Fact]
    public async Task HandleAsync_ForAuthenticatedSearch_IncludesOwnedPrivateMatches()
    {
        var repository = new PaginatedPostRepositoryStub(
            new PaginatedBlogPostReadResult(
            [
                BlogPost.Rehydrate(1, 2, 3, "Public blueprint", "Public summary", "Public content", isPublic: true, isAvailable: true),
                BlogPost.Rehydrate(2, 2, 3, "Private blueprint", "Owned summary", "Owned content", isPublic: false, isAvailable: true),
            ],
            totalCount: 2));
        var handler = new ListPublicPostsHandler(repository, new PostListCacheStub());

        var result = await handler.HandleAsync("blueprint", page: 1, pageSize: 6, requestingUserId: 2);

        Assert.Equal(2, result.Items.Count);
        Assert.Contains(result.Items, post => post.PostId == 2);
        Assert.Equal(2, repository.RequestedPage!.RequestingUserId);
    }

    [Fact]
    public async Task HandleAsync_ForAuthenticatedSearch_ExcludesPrivatePostsOwnedByOtherUsers()
    {
        var repository = new PaginatedPostRepositoryStub(
            new PaginatedBlogPostReadResult(
            [
                BlogPost.Rehydrate(1, 2, 3, "Owned blueprint", "Owned summary", "Owned content", isPublic: false, isAvailable: true),
                BlogPost.Rehydrate(2, 1, 3, "Other blueprint", "Other summary", "Other content", isPublic: false, isAvailable: true),
                BlogPost.Rehydrate(3, 1, 3, "Public blueprint", "Public summary", "Public content", isPublic: true, isAvailable: true),
            ],
            totalCount: 3));
        var handler = new ListPublicPostsHandler(repository, new PostListCacheStub());

        var result = await handler.HandleAsync("blueprint", page: 1, pageSize: 6, requestingUserId: 2);

        Assert.Equal(2, result.Items.Count);
        Assert.DoesNotContain(result.Items, post => post.PostId == 2);
        Assert.Contains(result.Items, post => post.PostId == 1);
        Assert.Contains(result.Items, post => post.PostId == 3);
    }
}
