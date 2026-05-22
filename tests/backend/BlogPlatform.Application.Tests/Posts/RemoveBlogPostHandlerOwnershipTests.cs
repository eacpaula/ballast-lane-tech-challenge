using BlogPlatform.Application.Abstractions;
using BlogPlatform.Application.Posts;
using BlogPlatform.Domain.Posts;

namespace BlogPlatform.Application.Tests.Posts;

public class RemoveBlogPostHandlerOwnershipTests
{
    [Fact]
    public async Task HandleAsync_WithNonOwner_ReturnsForbiddenFailureWithoutDeleting()
    {
        var existingPost = BlogPost.Rehydrate(
            id: 42,
            authorUserId: 8,
            categoryId: 3,
            title: "Original title",
            summary: "Original summary",
            content: "Original content");

        var postRepository = new TrackingPostRepository(existingPost);
        var handler = new RemoveBlogPostHandler(postRepository);

        var command = new RemoveBlogPostCommand(
            AuthenticatedUserId: 7,
            PostId: 42);

        var result = await handler.HandleAsync(command);

        Assert.False(result.IsSuccess);
        Assert.Equal("ForbiddenPostRemoval", result.ErrorCode);
        Assert.Equal("Users can remove only posts they own.", result.ErrorMessage);
        Assert.True(postRepository.GetByIdWasCalled);
        Assert.False(postRepository.DeleteWasCalled);
    }

    private sealed class TrackingPostRepository(BlogPost existingPost) : IPostRepository
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
            return Task.FromResult<BlogPost?>(existingPost);
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
            throw new NotSupportedException();
        }

        public Task<BlogPost> UpdateAsync(BlogPost post, CancellationToken cancellationToken = default)
        {
            throw new NotSupportedException();
        }
    }
}
