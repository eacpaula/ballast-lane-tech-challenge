using BlogPlatform.Application.Abstractions;
using BlogPlatform.Application.Posts;
using BlogPlatform.Domain.Posts;

namespace BlogPlatform.Application.Tests.Posts;

public sealed class ListOwnedPostsHandlerTests
{
    [Fact]
    public async Task HandleAsync_ReturnsPostsOwnedByAuthenticatedUser()
    {
        var repository = new TrackingPostRepository([
            BlogPost.Rehydrate(3, 7, 2, "Owned A", "summary", "content", isPublic: true, isAvailable: true),
            BlogPost.Rehydrate(4, 7, 3, "Owned B", null, "content", isPublic: false, isAvailable: true),
        ]);
        var handler = new ListOwnedPostsHandler(repository);

        var result = await handler.HandleAsync(7);

        Assert.Equal(2, result.Count);
        Assert.Collection(
            result,
            first =>
            {
                Assert.Equal(3, first.PostId);
                Assert.Equal("Owned A", first.Title);
            },
            second =>
            {
                Assert.Equal(4, second.PostId);
                Assert.False(second.IsPublic);
            });
    }

    [Fact]
    public async Task HandleAsync_UsesRepositoryAbstraction()
    {
        var repository = new TrackingPostRepository([]);
        var handler = new ListOwnedPostsHandler(repository);

        await handler.HandleAsync(11);

        Assert.Equal(11, repository.LastAuthorUserId);
    }

    private sealed class TrackingPostRepository(IReadOnlyList<BlogPost> posts) : IPostRepository
    {
        public int? LastAuthorUserId { get; private set; }

        public Task<IReadOnlyList<BlogPost>> ListByAuthorAsync(int authorUserId, CancellationToken cancellationToken = default)
        {
            LastAuthorUserId = authorUserId;
            return Task.FromResult(posts);
        }

        public Task<BlogPost> CreateAsync(BlogPost post, CancellationToken cancellationToken = default) => throw new NotSupportedException();
        public Task DeleteAsync(int postId, CancellationToken cancellationToken = default) => throw new NotSupportedException();
        public Task<BlogPost?> GetByIdAsync(int postId, CancellationToken cancellationToken = default) => throw new NotSupportedException();
        public Task<BlogPost?> GetByIdForAuthorAsync(int postId, int authorUserId, CancellationToken cancellationToken = default) => throw new NotSupportedException();
        public Task<BlogPost?> GetPublicReadByIdAsync(int postId, CancellationToken cancellationToken = default) => throw new NotSupportedException();
        public Task<IReadOnlyList<BlogPost>> ListPublicReadAsync(CancellationToken cancellationToken = default) => throw new NotSupportedException();
        public Task<BlogPost> UpdateAsync(BlogPost post, CancellationToken cancellationToken = default) => throw new NotSupportedException();
    }
}
