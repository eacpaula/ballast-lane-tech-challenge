using BlogPlatform.Application.Abstractions;
using BlogPlatform.Application.Posts;
using BlogPlatform.Domain.Posts;

namespace BlogPlatform.Application.Tests.Posts;

public class GetPublicPostByIdHandlerTests
{
    [Fact]
    public async Task HandleAsync_WithPublicAvailablePost_ReturnsSuccess()
    {
        var post = BlogPost.Rehydrate(
            id: 42,
            authorUserId: 7,
            categoryId: 3,
            title: "Public title",
            summary: "Visible summary",
            content: "Visible content",
            isPublic: true,
            isAvailable: true);

        var repository = new TrackingPostRepository(post);
        var handler = new GetPublicPostByIdHandler(repository);

        var result = await handler.HandleAsync(42);

        Assert.True(result.IsSuccess);
        Assert.Equal(42, result.PostId);
        Assert.Equal("Public title", result.Title);
        Assert.Equal("Visible summary", result.Summary);
        Assert.Equal("Visible content", result.Content);
        Assert.Null(result.ErrorCode);
        Assert.Null(result.ErrorMessage);
    }

    [Fact]
    public async Task HandleAsync_WhenPostDoesNotExist_ReturnsNotAvailable()
    {
        var repository = new TrackingPostRepository(null);
        var handler = new GetPublicPostByIdHandler(repository);

        var result = await handler.HandleAsync(42);

        Assert.False(result.IsSuccess);
        Assert.Equal("PostNotAvailable", result.ErrorCode);
        Assert.Equal("The requested post is not available to the public.", result.ErrorMessage);
    }

    [Fact]
    public async Task HandleAsync_WhenPostIsNotPublic_ReturnsNotAvailable()
    {
        var post = BlogPost.Rehydrate(
            id: 42,
            authorUserId: 7,
            categoryId: 3,
            title: "Private title",
            summary: "Hidden summary",
            content: "Hidden content",
            isPublic: false,
            isAvailable: true);

        var repository = new TrackingPostRepository(post);
        var handler = new GetPublicPostByIdHandler(repository);

        var result = await handler.HandleAsync(42);

        Assert.False(result.IsSuccess);
        Assert.Equal("PostNotAvailable", result.ErrorCode);
    }

    [Fact]
    public async Task HandleAsync_WhenPostIsUnavailable_ReturnsNotAvailable()
    {
        var post = BlogPost.Rehydrate(
            id: 42,
            authorUserId: 7,
            categoryId: 3,
            title: "Unavailable title",
            summary: "Hidden summary",
            content: "Hidden content",
            isPublic: true,
            isAvailable: false);

        var repository = new TrackingPostRepository(post);
        var handler = new GetPublicPostByIdHandler(repository);

        var result = await handler.HandleAsync(42);

        Assert.False(result.IsSuccess);
        Assert.Equal("PostNotAvailable", result.ErrorCode);
    }

    [Fact]
    public async Task HandleAsync_UsesRepositoryAbstractionToRetrieveThePost()
    {
        var post = BlogPost.Rehydrate(
            id: 42,
            authorUserId: 7,
            categoryId: 3,
            title: "Public title",
            summary: "Visible summary",
            content: "Visible content",
            isPublic: true,
            isAvailable: true);

        var repository = new TrackingPostRepository(post);
        var handler = new GetPublicPostByIdHandler(repository);

        await handler.HandleAsync(42);

        Assert.True(repository.GetByIdWasCalled);
        Assert.Equal(42, repository.RequestedPostId);
    }

    private sealed class TrackingPostRepository(BlogPost? post) : IPostRepository
    {
        public bool GetByIdWasCalled { get; private set; }

        public int? RequestedPostId { get; private set; }

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
            throw new NotSupportedException();
        }

        public Task<BlogPost?> GetPublicReadByIdAsync(int postId, CancellationToken cancellationToken = default)
        {
            GetByIdWasCalled = true;
            RequestedPostId = postId;
            return Task.FromResult(post?.Id == postId ? post : null);
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
