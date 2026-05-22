using BlogPlatform.Application.Abstractions;
using BlogPlatform.Application.Posts;
using BlogPlatform.Domain.Posts;

namespace BlogPlatform.Application.Tests.Posts;

public class RemoveBlogPostHandlerSuccessTests
{
    [Fact]
    public async Task HandleAsync_WithOwnedPost_RemovesPostAndReturnsSuccess()
    {
        var existingPost = BlogPost.Rehydrate(
            id: 42,
            authorUserId: 7,
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

        Assert.True(result.IsSuccess);
        Assert.Equal(42, result.PostId);
        Assert.Equal(7, result.AuthorUserId);
        Assert.Null(result.ErrorCode);
        Assert.Null(result.ErrorMessage);
        Assert.True(postRepository.DeleteWasCalled);
        Assert.Equal(42, postRepository.DeletedPostId);
    }

    private sealed class TrackingPostRepository(BlogPost existingPost) : IPostRepository
    {
        public bool DeleteWasCalled { get; private set; }

        public int? DeletedPostId { get; private set; }

        public Task<BlogPost> CreateAsync(BlogPost post, CancellationToken cancellationToken = default)
        {
            throw new NotSupportedException();
        }

        public Task DeleteAsync(int postId, CancellationToken cancellationToken = default)
        {
            DeleteWasCalled = true;
            DeletedPostId = postId;
            return Task.CompletedTask;
        }

        public Task<BlogPost?> GetByIdAsync(int postId, CancellationToken cancellationToken = default)
        {
            return Task.FromResult<BlogPost?>(existingPost.Id == postId ? existingPost : null);
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
