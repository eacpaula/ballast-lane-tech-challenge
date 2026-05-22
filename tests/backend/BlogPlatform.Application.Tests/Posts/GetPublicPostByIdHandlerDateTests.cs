using BlogPlatform.Application.Abstractions;
using BlogPlatform.Application.Posts;
using BlogPlatform.Domain.Posts;

namespace BlogPlatform.Application.Tests.Posts;

public class GetPublicPostByIdHandlerDateTests
{
    [Fact]
    public async Task HandleAsync_WhenPublishDateIsInFuture_ReturnsNotAvailable()
    {
        var post = BlogPost.Rehydrate(
            id: 10, authorUserId: 1, categoryId: 1,
            title: "Scheduled", summary: null, content: "content",
            isPublic: true, isAvailable: true,
            publishDate: DateTimeOffset.UtcNow.AddDays(7));

        var handler = new GetPublicPostByIdHandler(new StubPostRepository(post));

        var result = await handler.HandleAsync(10);

        Assert.False(result.IsSuccess);
        Assert.Equal("PostNotAvailable", result.ErrorCode);
    }

    [Fact]
    public async Task HandleAsync_WhenExpirationDateIsInPast_ReturnsNotAvailable()
    {
        var post = BlogPost.Rehydrate(
            id: 11, authorUserId: 1, categoryId: 1,
            title: "Expired", summary: null, content: "content",
            isPublic: true, isAvailable: true,
            expirationDate: DateTimeOffset.UtcNow.AddDays(-1));

        var handler = new GetPublicPostByIdHandler(new StubPostRepository(post));

        var result = await handler.HandleAsync(11);

        Assert.False(result.IsSuccess);
        Assert.Equal("PostNotAvailable", result.ErrorCode);
    }

    [Fact]
    public async Task HandleAsync_WhenPublishDateIsInPastAndNoExpiration_ReturnsSuccess()
    {
        var post = BlogPost.Rehydrate(
            id: 12, authorUserId: 1, categoryId: 1,
            title: "Active", summary: null, content: "content",
            isPublic: true, isAvailable: true,
            publishDate: DateTimeOffset.UtcNow.AddDays(-1));

        var handler = new GetPublicPostByIdHandler(new StubPostRepository(post));

        var result = await handler.HandleAsync(12);

        Assert.True(result.IsSuccess);
    }

    [Fact]
    public async Task HandleAsync_WhenBothDatesDefineActiveWindow_ReturnsSuccess()
    {
        var post = BlogPost.Rehydrate(
            id: 13, authorUserId: 1, categoryId: 1,
            title: "In window", summary: null, content: "content",
            isPublic: true, isAvailable: true,
            publishDate: DateTimeOffset.UtcNow.AddDays(-1),
            expirationDate: DateTimeOffset.UtcNow.AddDays(7));

        var handler = new GetPublicPostByIdHandler(new StubPostRepository(post));

        var result = await handler.HandleAsync(13);

        Assert.True(result.IsSuccess);
    }

    [Fact]
    public async Task HandleAsync_WhenNoDates_ReturnsSuccess()
    {
        var post = BlogPost.Rehydrate(
            id: 14, authorUserId: 1, categoryId: 1,
            title: "No dates", summary: null, content: "content",
            isPublic: true, isAvailable: true);

        var handler = new GetPublicPostByIdHandler(new StubPostRepository(post));

        var result = await handler.HandleAsync(14);

        Assert.True(result.IsSuccess);
    }

    private sealed class StubPostRepository(BlogPost? post) : IPostRepository
    {
        public Task<BlogPost?> GetPublicReadByIdAsync(int postId, CancellationToken cancellationToken = default)
            => Task.FromResult(post?.Id == postId ? post : null);

        public Task<BlogPost> CreateAsync(BlogPost post, CancellationToken cancellationToken = default) => throw new NotSupportedException();
        public Task DeleteAsync(int postId, CancellationToken cancellationToken = default) => throw new NotSupportedException();
        public Task<BlogPost?> GetByIdAsync(int postId, CancellationToken cancellationToken = default) => throw new NotSupportedException();
        public Task<BlogPost?> GetByIdForAuthorAsync(int postId, int authorUserId, CancellationToken cancellationToken = default) => throw new NotSupportedException();
        public Task<IReadOnlyList<BlogPost>> ListByAuthorAsync(int authorUserId, CancellationToken cancellationToken = default) => throw new NotSupportedException();
        public Task<IReadOnlyList<BlogPost>> ListPublicReadAsync(CancellationToken cancellationToken = default) => throw new NotSupportedException();
        public Task<IReadOnlyList<BlogPost>> SearchPublicReadAsync(string query, int? requestingUserId, CancellationToken cancellationToken = default) => throw new NotSupportedException();
        public Task<BlogPost> UpdateAsync(BlogPost post, CancellationToken cancellationToken = default) => throw new NotSupportedException();
    }
}
