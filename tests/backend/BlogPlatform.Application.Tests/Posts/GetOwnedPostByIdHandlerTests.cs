using BlogPlatform.Application.Abstractions;
using BlogPlatform.Application.Posts;
using BlogPlatform.Domain.Posts;

namespace BlogPlatform.Application.Tests.Posts;

public sealed class GetOwnedPostByIdHandlerTests
{
    [Fact]
    public async Task HandleAsync_ReturnsOwnedPost()
    {
        var repository = new TrackingPostRepository(BlogPost.Rehydrate(5, 7, 2, "Owned Post", "summary", "content", isPublic: false, isAvailable: true));
        var handler = new GetOwnedPostByIdHandler(repository);

        var result = await handler.HandleAsync(7, 5);

        Assert.True(result.IsSuccess);
        Assert.Equal(5, result.PostId);
        Assert.Equal(7, result.AuthorUserId);
        Assert.Equal(2, result.CategoryId);
        Assert.False(result.IsPublic!.Value);
    }

    [Fact]
    public async Task HandleAsync_ReturnsNotFoundWhenPostIsMissing()
    {
        var repository = new TrackingPostRepository(null);
        var handler = new GetOwnedPostByIdHandler(repository);

        var result = await handler.HandleAsync(7, 99);

        Assert.False(result.IsSuccess);
        Assert.Equal("PostNotFound", result.ErrorCode);
    }

    private sealed class TrackingPostRepository(BlogPost? post) : IPostRepository
    {
        public Task<BlogPost?> GetByIdForAuthorAsync(int postId, int authorUserId, CancellationToken cancellationToken = default)
            => Task.FromResult(post);

        public Task<BlogPost> CreateAsync(BlogPost post, CancellationToken cancellationToken = default) => throw new NotSupportedException();
        public Task DeleteAsync(int postId, CancellationToken cancellationToken = default) => throw new NotSupportedException();
        public Task<BlogPost?> GetByIdAsync(int postId, CancellationToken cancellationToken = default) => throw new NotSupportedException();
        public Task<BlogPost?> GetPublicReadByIdAsync(int postId, CancellationToken cancellationToken = default) => throw new NotSupportedException();
        public Task<IReadOnlyList<BlogPost>> ListByAuthorAsync(int authorUserId, CancellationToken cancellationToken = default) => throw new NotSupportedException();
        public Task<IReadOnlyList<BlogPost>> ListPublicReadAsync(CancellationToken cancellationToken = default) => throw new NotSupportedException();
        public Task<BlogPost> UpdateAsync(BlogPost post, CancellationToken cancellationToken = default) => throw new NotSupportedException();
    }
}
