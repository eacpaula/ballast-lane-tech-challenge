using BlogPlatform.Application.Abstractions;
using BlogPlatform.Application.Posts;
using BlogPlatform.Domain.Posts;

namespace BlogPlatform.Application.Tests.Posts;

public class CreateBlogPostHandlerValidationTests
{
    [Theory]
    [InlineData("", "Useful content")]
    [InlineData("   ", "Useful content")]
    [InlineData("Valid title", "")]
    [InlineData("Valid title", "   ")]
    public async Task HandleAsync_WithInvalidTitleOrContent_ReturnsValidationFailure(
        string title,
        string content)
    {
        var categoryRepository = new TrackingCategoryRepository(isAvailable: true);
        var postRepository = new TrackingPostRepository();
        var handler = new CreateBlogPostHandler(postRepository, categoryRepository);

        var command = new CreateBlogPostCommand(
            AuthenticatedUserId: 7,
            CategoryId: 3,
            Title: title,
            Summary: "Summary",
            Content: content);

        var result = await handler.HandleAsync(command);

        Assert.False(result.IsSuccess);
        Assert.Equal("ValidationError", result.ErrorCode);
        Assert.Contains("required", result.ErrorMessage, StringComparison.OrdinalIgnoreCase);
        Assert.Empty(postRepository.CreatedPosts);
    }

    [Fact]
    public async Task HandleAsync_WithMissingCategory_ReturnsCategoryFailure()
    {
        var categoryRepository = new TrackingCategoryRepository(isAvailable: false);
        var postRepository = new TrackingPostRepository();
        var handler = new CreateBlogPostHandler(postRepository, categoryRepository);

        var command = new CreateBlogPostCommand(
            AuthenticatedUserId: 7,
            CategoryId: 99,
            Title: "Valid title",
            Summary: "Summary",
            Content: "Useful content");

        var result = await handler.HandleAsync(command);

        Assert.False(result.IsSuccess);
        Assert.Equal("CategoryNotFoundOrUnavailable", result.ErrorCode);
        Assert.Empty(postRepository.CreatedPosts);
    }

    private sealed class TrackingCategoryRepository(bool isAvailable) : ICategoryRepository
    {
        public Task<bool> ExistsAndAvailableAsync(int categoryId, CancellationToken cancellationToken = default)
        {
            return Task.FromResult(isAvailable);
        }
    }

    private sealed class TrackingPostRepository : IPostRepository
    {
        public List<BlogPost> CreatedPosts { get; } = [];

        public Task<BlogPost> CreateAsync(BlogPost post, CancellationToken cancellationToken = default)
        {
            CreatedPosts.Add(post);
            return Task.FromResult(post);
        }
    }
}
