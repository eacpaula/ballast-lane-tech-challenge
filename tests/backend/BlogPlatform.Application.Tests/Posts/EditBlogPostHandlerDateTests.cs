using BlogPlatform.Application.Abstractions;
using BlogPlatform.Application.Posts;
using BlogPlatform.Domain.Posts;

namespace BlogPlatform.Application.Tests.Posts;

public class EditBlogPostHandlerDateTests
{
    private static readonly DateTimeOffset FutureDate = DateTimeOffset.UtcNow.AddDays(30);
    private static readonly DateTimeOffset AnotherFutureDate = DateTimeOffset.UtcNow.AddDays(60);

    [Fact]
    public async Task HandleAsync_WithValidDates_ResultCarriesUpdatedDates()
    {
        var existingPost = BlogPost.Rehydrate(
            id: 42, authorUserId: 7, categoryId: 3,
            title: "Original", summary: null, content: "Content");

        var handler = new EditBlogPostHandler(new FakePostRepository(existingPost));

        var command = new EditBlogPostCommand(
            AuthenticatedUserId: 7,
            PostId: 42,
            Title: "Updated",
            Summary: null,
            Content: "Content",
            PublishDate: FutureDate,
            ExpirationDate: AnotherFutureDate);

        var result = await handler.HandleAsync(command);

        Assert.True(result.IsSuccess);
        Assert.Equal(FutureDate, result.PublishDate);
        Assert.Equal(AnotherFutureDate, result.ExpirationDate);
    }

    [Fact]
    public async Task HandleAsync_WithExpirationBeforePublish_ReturnsValidationError()
    {
        var existingPost = BlogPost.Rehydrate(
            id: 42, authorUserId: 7, categoryId: 3,
            title: "Original", summary: null, content: "Content");

        var handler = new EditBlogPostHandler(new FakePostRepository(existingPost));

        var command = new EditBlogPostCommand(
            AuthenticatedUserId: 7,
            PostId: 42,
            Title: "Updated",
            Summary: null,
            Content: "Content",
            PublishDate: AnotherFutureDate,
            ExpirationDate: FutureDate);

        var result = await handler.HandleAsync(command);

        Assert.False(result.IsSuccess);
        Assert.Equal("expirationDate", result.ErrorCode);
    }

    [Fact]
    public async Task HandleAsync_WithNullDates_ResultHasNullDates()
    {
        var existingPost = BlogPost.Rehydrate(
            id: 42, authorUserId: 7, categoryId: 3,
            title: "Original", summary: null, content: "Content",
            publishDate: FutureDate, expirationDate: AnotherFutureDate);

        var handler = new EditBlogPostHandler(new FakePostRepository(existingPost));

        var command = new EditBlogPostCommand(
            AuthenticatedUserId: 7,
            PostId: 42,
            Title: "Updated",
            Summary: null,
            Content: "Content",
            PublishDate: null,
            ExpirationDate: null);

        var result = await handler.HandleAsync(command);

        Assert.True(result.IsSuccess);
        Assert.Null(result.PublishDate);
        Assert.Null(result.ExpirationDate);
    }

    private sealed class FakePostRepository(BlogPost existingPost) : IPostRepository
    {
        public Task<BlogPost?> GetByIdAsync(int postId, CancellationToken cancellationToken = default)
            => Task.FromResult<BlogPost?>(existingPost.Id == postId ? existingPost : null);

        public Task<BlogPost> UpdateAsync(BlogPost post, CancellationToken cancellationToken = default)
            => Task.FromResult(post);

        public Task<BlogPost> CreateAsync(BlogPost post, CancellationToken cancellationToken = default) => throw new NotSupportedException();
        public Task DeleteAsync(int postId, CancellationToken cancellationToken = default) => throw new NotSupportedException();
        public Task<BlogPost?> GetByIdForAuthorAsync(int postId, int authorUserId, CancellationToken cancellationToken = default) => throw new NotSupportedException();
        public Task<BlogPost?> GetPublicReadByIdAsync(int postId, CancellationToken cancellationToken = default) => throw new NotSupportedException();
        public Task<IReadOnlyList<BlogPost>> ListByAuthorAsync(int authorUserId, CancellationToken cancellationToken = default) => throw new NotSupportedException();
        public Task<IReadOnlyList<BlogPost>> ListPublicReadAsync(CancellationToken cancellationToken = default) => throw new NotSupportedException();
        public Task<IReadOnlyList<BlogPost>> SearchPublicReadAsync(string query, int? requestingUserId, CancellationToken cancellationToken = default) => throw new NotSupportedException();
    }
}
