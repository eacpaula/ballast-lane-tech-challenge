using BlogPlatform.Application.Abstractions;
using BlogPlatform.Application.Posts;
using BlogPlatform.Domain.Posts;

namespace BlogPlatform.Application.Tests.Posts;

public class EditBlogPostHandlerSuccessTests
{
    [Fact]
    public async Task HandleAsync_WithOwnedPostAndValidInput_UpdatesPostAndReturnsSuccess()
    {
        var existingPost = BlogPost.Rehydrate(
            id: 42,
            authorUserId: 7,
            categoryId: 3,
            title: "Original title",
            summary: "Original summary",
            content: "Original content");

        var postRepository = new FakePostRepository(existingPost);
        var handler = new EditBlogPostHandler(postRepository);

        var command = new EditBlogPostCommand(
            AuthenticatedUserId: 7,
            PostId: 42,
            Title: "Updated title",
            Summary: "Updated summary",
            Content: "Updated content");

        var result = await handler.HandleAsync(command);

        Assert.True(result.IsSuccess);
        Assert.Equal(42, result.PostId);
        Assert.Equal(7, result.AuthorUserId);
        Assert.Equal("Updated title", result.Title);
        Assert.Equal("Updated summary", result.Summary);
        Assert.Equal("Updated content", result.Content);
        Assert.NotNull(postRepository.UpdatedPost);
        Assert.Equal("Updated title", postRepository.UpdatedPost!.Title);
    }

    private sealed class FakePostRepository(BlogPost existingPost) : IPostRepository
    {
        public BlogPost? UpdatedPost { get; private set; }

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
            UpdatedPost = post;
            return Task.FromResult(post);
        }
    }
}
