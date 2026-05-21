using BlogPlatform.Application.Abstractions;
using BlogPlatform.Application.Posts;
using BlogPlatform.Domain.Posts;
using BlogPlatform.Domain.Reactions;

namespace BlogPlatform.Application.Tests.Reactions;

public class ReactToPostHandlerValidationTests
{
    [Fact]
    public async Task HandleAsync_WithInvalidReactionType_ReturnsValidationFailure()
    {
        var postRepository = new TrackingPostRepository();
        var reactionRepository = new TrackingReactionRepository();
        var handler = new ReactToPostHandler(postRepository, reactionRepository);

        var command = new ReactToPostCommand(
            PostId: 42,
            ReactionType: "love",
            AuthenticatedUserId: null,
            VisitorIdentifier: "visitor-1");

        var result = await handler.HandleAsync(command);

        Assert.False(result.IsSuccess);
        Assert.Equal("InvalidReactionType", result.ErrorCode);
        Assert.Equal("The submitted reaction type is not supported.", result.ErrorMessage);
        Assert.False(postRepository.GetByIdWasCalled);
        Assert.False(reactionRepository.CreateWasCalled);
    }

    [Fact]
    public async Task HandleAsync_WithInvalidActor_ReturnsValidationFailure()
    {
        var postRepository = new TrackingPostRepository();
        var reactionRepository = new TrackingReactionRepository();
        var handler = new ReactToPostHandler(postRepository, reactionRepository);

        var command = new ReactToPostCommand(
            PostId: 42,
            ReactionType: "like",
            AuthenticatedUserId: null,
            VisitorIdentifier: "   ");

        var result = await handler.HandleAsync(command);

        Assert.False(result.IsSuccess);
        Assert.Equal("InvalidReactionActor", result.ErrorCode);
        Assert.Equal("A valid user or visitor identity is required to react to a post.", result.ErrorMessage);
        Assert.False(postRepository.GetByIdWasCalled);
        Assert.False(reactionRepository.CreateWasCalled);
    }

    private sealed class TrackingPostRepository : IPostRepository
    {
        public bool GetByIdWasCalled { get; private set; }

        public Task<BlogPost> CreateAsync(BlogPost post, CancellationToken cancellationToken = default) => throw new NotSupportedException();
        public Task DeleteAsync(int postId, CancellationToken cancellationToken = default) => throw new NotSupportedException();
        public Task<BlogPost?> GetByIdAsync(int postId, CancellationToken cancellationToken = default)
        {
            GetByIdWasCalled = true;
            return Task.FromResult<BlogPost?>(null);
        }

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
