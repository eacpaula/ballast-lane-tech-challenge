using BlogPlatform.Application.Abstractions;
using BlogPlatform.Application.Posts;
using BlogPlatform.Domain.Posts;

namespace BlogPlatform.Application.Tests.Posts;

public class ListPublicPostsHandlerTests
{
    [Fact]
    public async Task HandleAsync_WithoutAuthenticationContext_ReturnsPublicAvailablePosts()
    {
        var posts = new[]
        {
            BlogPost.Rehydrate(
                id: 1,
                authorUserId: 7,
                categoryId: 3,
                title: "Public title",
                summary: "Visible summary",
                content: "Visible content",
                isPublic: true,
                isAvailable: true),
        };

        var repository = new TrackingPostRepository(posts);
        var handler = new ListPublicPostsHandler(repository);

        var result = await handler.HandleAsync();

        Assert.Single(result);
        Assert.Equal(1, result[0].PostId);
        Assert.Equal("Public title", result[0].Title);
        Assert.Equal("Visible summary", result[0].Summary);
    }

    [Fact]
    public async Task HandleAsync_ExcludesNonPublicPosts()
    {
        var posts = new[]
        {
            BlogPost.Rehydrate(1, 7, 3, "Public title", "Visible summary", "Visible content", isPublic: true, isAvailable: true),
            BlogPost.Rehydrate(2, 8, 3, "Private title", "Hidden summary", "Hidden content", isPublic: false, isAvailable: true),
        };

        var repository = new TrackingPostRepository(posts);
        var handler = new ListPublicPostsHandler(repository);

        var result = await handler.HandleAsync();

        Assert.Single(result);
        Assert.DoesNotContain(result, post => post.PostId == 2);
    }

    [Fact]
    public async Task HandleAsync_ExcludesUnavailablePosts()
    {
        var posts = new[]
        {
            BlogPost.Rehydrate(1, 7, 3, "Public title", "Visible summary", "Visible content", isPublic: true, isAvailable: true),
            BlogPost.Rehydrate(3, 8, 3, "Unavailable title", "Hidden summary", "Hidden content", isPublic: true, isAvailable: false),
        };

        var repository = new TrackingPostRepository(posts);
        var handler = new ListPublicPostsHandler(repository);

        var result = await handler.HandleAsync();

        Assert.Single(result);
        Assert.DoesNotContain(result, post => post.PostId == 3);
    }

    [Fact]
    public async Task HandleAsync_UsesRepositoryAbstractionToRetrievePosts()
    {
        var repository = new TrackingPostRepository([]);
        var handler = new ListPublicPostsHandler(repository);

        await handler.HandleAsync();

        Assert.True(repository.ListWasCalled);
    }

    private sealed class TrackingPostRepository(IReadOnlyList<BlogPost> posts) : IPostRepository
    {
        public bool ListWasCalled { get; private set; }

        public Task<BlogPost> CreateAsync(BlogPost post, CancellationToken cancellationToken = default)
        {
            throw new NotSupportedException();
        }

        public Task DeleteAsync(int postId, CancellationToken cancellationToken = default)
        {
            throw new NotSupportedException();
        }

        public Task<BlogPost?> GetByIdAsync(int postId, CancellationToken cancellationToken = default)
        {
            throw new NotSupportedException();
        }

        public Task<BlogPost?> GetByIdForAuthorAsync(int postId, int authorUserId, CancellationToken cancellationToken = default)
        {
            throw new NotSupportedException();
        }

        public Task<BlogPost?> GetPublicReadByIdAsync(int postId, CancellationToken cancellationToken = default)
        {
            throw new NotSupportedException();
        }

        public Task<IReadOnlyList<BlogPost>> ListByAuthorAsync(int authorUserId, CancellationToken cancellationToken = default)
        {
            throw new NotSupportedException();
        }

        public Task<IReadOnlyList<BlogPost>> ListPublicReadAsync(CancellationToken cancellationToken = default)
        {
            ListWasCalled = true;
            return Task.FromResult(posts);
        }

        public Task<BlogPost> UpdateAsync(BlogPost post, CancellationToken cancellationToken = default)
        {
            throw new NotSupportedException();
        }
    }
}
