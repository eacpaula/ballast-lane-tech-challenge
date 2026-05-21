using BlogPlatform.Application.Abstractions;
using BlogPlatform.Application.Posts;
using BlogPlatform.Domain.Posts;
using BlogPlatform.Domain.Reactions;

namespace BlogPlatform.Application.Tests.Reactions;

public class ReactToPostHandlerPostAvailabilityTests
{
    [Fact]
    public async Task HandleAsync_WithMissingPost_ReturnsNotAvailableFailure()
    {
        var postRepository = new TrackingPostRepository(null);
        var reactionRepository = new TrackingReactionRepository();
        var handler = new ReactToPostHandler(postRepository, reactionRepository);

        var command = new ReactToPostCommand(42, "like", null, "visitor-1");

        var result = await handler.HandleAsync(command);

        Assert.False(result.IsSuccess);
        Assert.Equal("PostNotAvailable", result.ErrorCode);
        Assert.False(reactionRepository.CreateWasCalled);
    }

    [Fact]
    public async Task HandleAsync_WithNonPublicPost_ReturnsNotAvailableFailure()
    {
        var post = BlogPost.Rehydrate(42, 7, 3, "Title", "Summary", "Content", isPublic: false, isAvailable: true);
        var postRepository = new TrackingPostRepository(post);
        var reactionRepository = new TrackingReactionRepository();
        var handler = new ReactToPostHandler(postRepository, reactionRepository);

        var result = await handler.HandleAsync(new ReactToPostCommand(42, "like", null, "visitor-1"));

        Assert.False(result.IsSuccess);
        Assert.Equal("PostNotAvailable", result.ErrorCode);
        Assert.False(reactionRepository.CreateWasCalled);
    }

    [Fact]
    public async Task HandleAsync_WithUnavailablePost_ReturnsNotAvailableFailure()
    {
        var post = BlogPost.Rehydrate(42, 7, 3, "Title", "Summary", "Content", isPublic: true, isAvailable: false);
        var postRepository = new TrackingPostRepository(post);
        var reactionRepository = new TrackingReactionRepository();
        var handler = new ReactToPostHandler(postRepository, reactionRepository);

        var result = await handler.HandleAsync(new ReactToPostCommand(42, "like", null, "visitor-1"));

        Assert.False(result.IsSuccess);
        Assert.Equal("PostNotAvailable", result.ErrorCode);
        Assert.Equal("The requested post is not available for public reactions.", result.ErrorMessage);
        Assert.False(reactionRepository.CreateWasCalled);
    }

    private sealed class TrackingPostRepository(BlogPost? post) : IPostRepository
    {
        public Task<BlogPost> CreateAsync(BlogPost post, CancellationToken cancellationToken = default) => throw new NotSupportedException();
        public Task DeleteAsync(int postId, CancellationToken cancellationToken = default) => throw new NotSupportedException();
        public Task<BlogPost?> GetByIdAsync(int postId, CancellationToken cancellationToken = default) => Task.FromResult(post?.Id == postId ? post : null);
        public Task<BlogPost?> GetPublicReadByIdAsync(int postId, CancellationToken cancellationToken = default) => throw new NotSupportedException();
        public Task<IReadOnlyList<BlogPost>> ListPublicReadAsync(CancellationToken cancellationToken = default) => throw new NotSupportedException();
        public Task<BlogPost> UpdateAsync(BlogPost post, CancellationToken cancellationToken = default) => throw new NotSupportedException();
    }

    private sealed class TrackingReactionRepository : IPostReactionRepository
    {
        public bool CreateWasCalled { get; private set; }

        public Task<PostReaction> CreateAsync(PostReaction reaction, CancellationToken cancellationToken = default)
        {
            CreateWasCalled = true;
            return Task.FromResult(reaction);
        }
    }
}
