using BlogPlatform.Application.Abstractions;
using BlogPlatform.Application.Posts;
using BlogPlatform.Domain.Posts;

namespace BlogPlatform.Application.Tests.Posts;

public class EditBlogPostHandlerAuthenticationTests
{
    [Fact]
    public async Task HandleAsync_WithMissingAuthenticatedUser_ReturnsAuthenticationFailure()
    {
        var postRepository = new TrackingPostRepository();
        var handler = new EditBlogPostHandler(postRepository);

        var command = new EditBlogPostCommand(
            AuthenticatedUserId: 0,
            PostId: 42,
            Title: "Updated title",
            Summary: "Updated summary",
            Content: "Updated content");

        var result = await handler.HandleAsync(command);

        Assert.False(result.IsSuccess);
        Assert.Equal("AuthenticationRequired", result.ErrorCode);
        Assert.False(postRepository.GetByIdWasCalled);
        Assert.False(postRepository.UpdateWasCalled);
    }

    private sealed class TrackingPostRepository : IPostRepository
    {
        public bool GetByIdWasCalled { get; private set; }
        public bool UpdateWasCalled { get; private set; }

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
            GetByIdWasCalled = true;
            return Task.FromResult<BlogPost?>(null);
        }

        public Task<BlogPost?> GetPublicReadByIdAsync(int postId, CancellationToken cancellationToken = default)
        {
            throw new NotSupportedException();
        }

        public Task<IReadOnlyList<BlogPost>> ListPublicReadAsync(CancellationToken cancellationToken = default)
        {
            throw new NotSupportedException();
        }

        public Task<BlogPost> UpdateAsync(BlogPost post, CancellationToken cancellationToken = default)
        {
            UpdateWasCalled = true;
            return Task.FromResult(post);
        }
    }
}
