using BlogPlatform.Application.Abstractions;
using BlogPlatform.Application.Posts;
using BlogPlatform.Domain.Posts;

namespace BlogPlatform.Application.Tests.Posts;

public class RemoveBlogPostHandlerAuthenticationTests
{
    [Fact]
    public async Task HandleAsync_WithMissingAuthenticatedUser_ReturnsAuthenticationFailureWithoutLoadingOrDeleting()
    {
        var postRepository = new TrackingPostRepository();
        var handler = new RemoveBlogPostHandler(postRepository);

        var command = new RemoveBlogPostCommand(
            AuthenticatedUserId: 0,
            PostId: 42);

        var result = await handler.HandleAsync(command);

        Assert.False(result.IsSuccess);
        Assert.Null(result.PostId);
        Assert.Null(result.AuthorUserId);
        Assert.Equal("AuthenticationRequired", result.ErrorCode);
        Assert.Equal("An authenticated user is required to remove a post.", result.ErrorMessage);
        Assert.False(postRepository.GetByIdWasCalled);
        Assert.False(postRepository.DeleteWasCalled);
    }

    private sealed class TrackingPostRepository : IPostRepository
    {
        public bool GetByIdWasCalled { get; private set; }

        public bool DeleteWasCalled { get; private set; }

        public Task<BlogPost> CreateAsync(BlogPost post, CancellationToken cancellationToken = default)
        {
            throw new NotSupportedException();
        }

        public Task DeleteAsync(int postId, CancellationToken cancellationToken = default)
        {
            DeleteWasCalled = true;
            return Task.CompletedTask;
        }

        public Task<BlogPost?> GetByIdAsync(int postId, CancellationToken cancellationToken = default)
        {
            GetByIdWasCalled = true;
            return Task.FromResult<BlogPost?>(null);
        }

        public Task<BlogPost> UpdateAsync(BlogPost post, CancellationToken cancellationToken = default)
        {
            throw new NotSupportedException();
        }
    }
}
