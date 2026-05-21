using BlogPlatform.Application.Abstractions;
using BlogPlatform.Application.Posts;
using BlogPlatform.Domain.Posts;

namespace BlogPlatform.Application.Tests.Posts;

public class CreateBlogPostHandlerAuthenticationTests
{
    [Fact]
    public async Task HandleAsync_WithMissingAuthenticatedUser_ReturnsAuthenticationFailure()
    {
        var categoryRepository = new TrackingCategoryRepository();
        var postRepository = new TrackingPostRepository();
        var handler = new CreateBlogPostHandler(postRepository, categoryRepository);

        var command = new CreateBlogPostCommand(
            AuthenticatedUserId: 0,
            CategoryId: 3,
            Title: "Valid title",
            Summary: "Summary",
            Content: "Useful content");

        var result = await handler.HandleAsync(command);

        Assert.False(result.IsSuccess);
        Assert.Equal("AuthenticationRequired", result.ErrorCode);
        Assert.False(categoryRepository.WasCalled);
        Assert.Empty(postRepository.CreatedPosts);
    }

    private sealed class TrackingCategoryRepository : ICategoryRepository
    {
        public bool WasCalled { get; private set; }

        public Task<bool> ExistsAndAvailableAsync(int categoryId, CancellationToken cancellationToken = default)
        {
            WasCalled = true;
            return Task.FromResult(true);
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

        public Task DeleteAsync(int postId, CancellationToken cancellationToken = default)
        {
            throw new NotSupportedException();
        }

        public Task<BlogPost?> GetByIdAsync(int postId, CancellationToken cancellationToken = default)
        {
            return Task.FromResult<BlogPost?>(null);
        }

        public Task<BlogPost> UpdateAsync(BlogPost post, CancellationToken cancellationToken = default)
        {
            return Task.FromResult(post);
        }
    }
}
