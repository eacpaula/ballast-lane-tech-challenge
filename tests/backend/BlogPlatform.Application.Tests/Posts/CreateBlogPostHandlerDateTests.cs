using BlogPlatform.Application.Abstractions;
using BlogPlatform.Application.Posts;
using BlogPlatform.Domain.Categories;
using BlogPlatform.Domain.Posts;

namespace BlogPlatform.Application.Tests.Posts;

public class CreateBlogPostHandlerDateTests
{
    private static readonly DateTimeOffset FutureDate = DateTimeOffset.UtcNow.AddDays(30);
    private static readonly DateTimeOffset PastDate = DateTimeOffset.UtcNow.AddDays(-30);
    private static readonly DateTimeOffset AnotherFutureDate = DateTimeOffset.UtcNow.AddDays(60);

    [Fact]
    public async Task HandleAsync_WithOnlyPublishDate_ResultCarriesPublishDate()
    {
        var handler = CreateHandler();

        var command = new CreateBlogPostCommand(
            AuthenticatedUserId: 7,
            CategoryId: 3,
            Title: "Scheduled post",
            Summary: null,
            Content: "Content",
            PublishDate: FutureDate);

        var result = await handler.HandleAsync(command);

        Assert.True(result.IsSuccess);
        Assert.Equal(FutureDate, result.PublishDate);
        Assert.Null(result.ExpirationDate);
    }

    [Fact]
    public async Task HandleAsync_WithOnlyExpirationDate_ResultCarriesExpirationDate()
    {
        var handler = CreateHandler();

        var command = new CreateBlogPostCommand(
            AuthenticatedUserId: 7,
            CategoryId: 3,
            Title: "Post with expiry",
            Summary: null,
            Content: "Content",
            ExpirationDate: FutureDate);

        var result = await handler.HandleAsync(command);

        Assert.True(result.IsSuccess);
        Assert.Null(result.PublishDate);
        Assert.Equal(FutureDate, result.ExpirationDate);
    }

    [Fact]
    public async Task HandleAsync_WithBothDatesWhereExpirationAfterPublish_Succeeds()
    {
        var handler = CreateHandler();

        var command = new CreateBlogPostCommand(
            AuthenticatedUserId: 7,
            CategoryId: 3,
            Title: "Post with both dates",
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
        var handler = CreateHandler();

        var command = new CreateBlogPostCommand(
            AuthenticatedUserId: 7,
            CategoryId: 3,
            Title: "Invalid dates",
            Summary: null,
            Content: "Content",
            PublishDate: AnotherFutureDate,
            ExpirationDate: FutureDate);

        var result = await handler.HandleAsync(command);

        Assert.False(result.IsSuccess);
        Assert.Equal("expirationDate", result.ErrorCode);
    }

    [Fact]
    public async Task HandleAsync_WithExpirationEqualToPublish_ReturnsValidationError()
    {
        var handler = CreateHandler();

        var command = new CreateBlogPostCommand(
            AuthenticatedUserId: 7,
            CategoryId: 3,
            Title: "Equal dates",
            Summary: null,
            Content: "Content",
            PublishDate: FutureDate,
            ExpirationDate: FutureDate);

        var result = await handler.HandleAsync(command);

        Assert.False(result.IsSuccess);
        Assert.Equal("expirationDate", result.ErrorCode);
    }

    [Fact]
    public async Task HandleAsync_WithNoDates_ResultHasNullDates()
    {
        var handler = CreateHandler();

        var command = new CreateBlogPostCommand(
            AuthenticatedUserId: 7,
            CategoryId: 3,
            Title: "Post without dates",
            Summary: null,
            Content: "Content");

        var result = await handler.HandleAsync(command);

        Assert.True(result.IsSuccess);
        Assert.Null(result.PublishDate);
        Assert.Null(result.ExpirationDate);
    }

    private static CreateBlogPostHandler CreateHandler()
    {
        var categoryRepository = new FakeCategoryRepository();
        var postRepository = new FakePostRepository();
        return new CreateBlogPostHandler(postRepository, categoryRepository);
    }

    private sealed class FakeCategoryRepository : ICategoryRepository
    {
        public Task<bool> ExistsAndAvailableAsync(int categoryId, CancellationToken cancellationToken = default)
            => Task.FromResult(true);

        public Task<PaginatedCategoryResult<PostCategory>> ListAllAsync(CategoryPageRequest request, CancellationToken cancellationToken = default)
            => throw new NotSupportedException();

        public Task<PaginatedCategoryResult<PostCategory>> ListAvailableAsync(CategoryPageRequest request, CancellationToken cancellationToken = default)
            => throw new NotSupportedException();

        public Task<PostCategory> CreateAsync(PostCategory category, CancellationToken cancellationToken = default)
            => throw new NotSupportedException();

        public Task<PostCategory> DeactivateAsync(PostCategory category, CancellationToken cancellationToken = default)
            => throw new NotSupportedException();

        public Task<PostCategory?> GetByIdAsync(int categoryId, CancellationToken cancellationToken = default)
            => throw new NotSupportedException();

        public Task<bool> TitleExistsAsync(string title, int? excludingCategoryId = null, CancellationToken cancellationToken = default)
            => throw new NotSupportedException();

        public Task<PostCategory> UpdateAsync(PostCategory category, CancellationToken cancellationToken = default)
            => throw new NotSupportedException();
    }

    private sealed class FakePostRepository : IPostRepository
    {
        public Task<BlogPost> CreateAsync(BlogPost post, CancellationToken cancellationToken = default)
        {
            var saved = BlogPost.Rehydrate(
                id: 101,
                authorUserId: post.AuthorUserId,
                categoryId: post.CategoryId,
                title: post.Title,
                summary: post.Summary,
                content: post.Content,
                publishDate: post.PublishDate,
                expirationDate: post.ExpirationDate);
            return Task.FromResult(saved);
        }

        public Task DeleteAsync(int postId, CancellationToken cancellationToken = default) => throw new NotSupportedException();
        public Task<BlogPost?> GetByIdAsync(int postId, CancellationToken cancellationToken = default) => Task.FromResult<BlogPost?>(null);
        public Task<BlogPost?> GetByIdForAuthorAsync(int postId, int authorUserId, CancellationToken cancellationToken = default) => throw new NotSupportedException();
        public Task<BlogPost?> GetPublicReadByIdAsync(int postId, CancellationToken cancellationToken = default) => throw new NotSupportedException();
        public Task<IReadOnlyList<BlogPost>> ListByAuthorAsync(int authorUserId, CancellationToken cancellationToken = default) => throw new NotSupportedException();
        public Task<IReadOnlyList<BlogPost>> ListPublicReadAsync(CancellationToken cancellationToken = default) => throw new NotSupportedException();
        public Task<IReadOnlyList<BlogPost>> SearchPublicReadAsync(string query, int? requestingUserId, CancellationToken cancellationToken = default) => throw new NotSupportedException();
        public Task<BlogPost> UpdateAsync(BlogPost post, CancellationToken cancellationToken = default) => Task.FromResult(post);
    }
}
