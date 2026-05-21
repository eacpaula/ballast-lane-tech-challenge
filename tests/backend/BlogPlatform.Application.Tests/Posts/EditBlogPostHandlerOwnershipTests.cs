using BlogPlatform.Application.Abstractions;
using BlogPlatform.Application.Posts;
using BlogPlatform.Domain.Posts;

namespace BlogPlatform.Application.Tests.Posts;

public class EditBlogPostHandlerOwnershipTests
{
    [Fact]
    public async Task HandleAsync_WithNonOwner_ReturnsForbiddenFailure()
    {
        var existingPost = BlogPost.Rehydrate(
            id: 42,
            authorUserId: 8,
            categoryId: 3,
            title: "Original title",
            summary: "Original summary",
            content: "Original content");

        var postRepository = new TrackingPostRepository(existingPost);
        var handler = new EditBlogPostHandler(postRepository);

        var command = new EditBlogPostCommand(
            AuthenticatedUserId: 7,
            PostId: 42,
            Title: "Updated title",
            Summary: "Updated summary",
            Content: "Updated content");

        var result = await handler.HandleAsync(command);

        Assert.False(result.IsSuccess);
        Assert.Equal("ForbiddenPostEdit", result.ErrorCode);
        Assert.False(postRepository.UpdateWasCalled);
    }

    private sealed class TrackingPostRepository(BlogPost existingPost) : IPostRepository
    {
        public bool UpdateWasCalled { get; private set; }

        public Task<BlogPost> CreateAsync(BlogPost post, CancellationToken cancellationToken = default)
        {
            throw new NotSupportedException();
        }

        public Task<BlogPost?> GetByIdAsync(int postId, CancellationToken cancellationToken = default)
        {
            return Task.FromResult<BlogPost?>(existingPost);
        }

        public Task<BlogPost> UpdateAsync(BlogPost post, CancellationToken cancellationToken = default)
        {
            UpdateWasCalled = true;
            return Task.FromResult(post);
        }
    }
}
