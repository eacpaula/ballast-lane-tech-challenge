using BlogPlatform.Application.Abstractions;
using BlogPlatform.Application.Posts;
using BlogPlatform.Domain.Posts;

namespace BlogPlatform.Application.Tests.Posts;

public class RemoveBlogPostHandlerMissingPostTests
{
    [Fact]
    public async Task HandleAsync_WithMissingPost_ReturnsPostNotFoundFailure()
    {
        var postRepository = new MissingPostRepository();
        var handler = new RemoveBlogPostHandler(postRepository);

        var command = new RemoveBlogPostCommand(
            AuthenticatedUserId: 7,
            PostId: 42);

        var result = await handler.HandleAsync(command);

        Assert.False(result.IsSuccess);
        Assert.Null(result.PostId);
        Assert.Null(result.AuthorUserId);
        Assert.Equal("PostNotFound", result.ErrorCode);
        Assert.Equal("The requested post was not found.", result.ErrorMessage);
    }

    private sealed class MissingPostRepository : IPostRepository
    {
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
            return Task.FromResult<BlogPost?>(null);
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
