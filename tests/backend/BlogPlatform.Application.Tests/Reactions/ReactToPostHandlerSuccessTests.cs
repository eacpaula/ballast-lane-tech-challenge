using BlogPlatform.Application.Abstractions;
using BlogPlatform.Application.Posts;
using BlogPlatform.Domain.Posts;
using BlogPlatform.Domain.Reactions;

namespace BlogPlatform.Application.Tests.Reactions;

public class ReactToPostHandlerSuccessTests
{
    [Fact]
    public async Task HandleAsync_WithPublicAvailablePostAndLikeReaction_PersistsReactionAndReturnsSuccess()
    {
        var post = BlogPost.Rehydrate(
            id: 42,
            authorUserId: 7,
            categoryId: 3,
            title: "Public post",
            summary: "Summary",
            content: "Content",
            isPublic: true,
            isAvailable: true);

        var postRepository = new TrackingPostRepository(post);
        var reactionRepository = new TrackingReactionRepository();
        var handler = new ReactToPostHandler(postRepository, reactionRepository);

        var command = new ReactToPostCommand(
            PostId: 42,
            ReactionType: "like",
            AuthenticatedUserId: null,
            VisitorIdentifier: "visitor-1");

        var result = await handler.HandleAsync(command);

        Assert.True(result.IsSuccess);
        Assert.Equal(42, result.PostId);
        Assert.Equal("Like", result.ReactionType);
        Assert.Null(result.UserId);
        Assert.Equal("visitor-1", result.VisitorIdentifier);
        Assert.Null(result.ErrorCode);
        Assert.True(reactionRepository.CreateWasCalled);
        Assert.NotNull(reactionRepository.CreatedReaction);
        Assert.Equal("Like", reactionRepository.CreatedReaction!.ReactionType.Value);
    }

    [Fact]
    public async Task HandleAsync_WithPublicAvailablePostAndDislikeReaction_PersistsReactionAndReturnsSuccess()
    {
        var post = BlogPost.Rehydrate(
            id: 42,
            authorUserId: 7,
            categoryId: 3,
            title: "Public post",
            summary: "Summary",
            content: "Content",
            isPublic: true,
            isAvailable: true);

        var postRepository = new TrackingPostRepository(post);
        var reactionRepository = new TrackingReactionRepository();
        var handler = new ReactToPostHandler(postRepository, reactionRepository);

        var command = new ReactToPostCommand(
            PostId: 42,
            ReactionType: "dislike",
            AuthenticatedUserId: 9,
            VisitorIdentifier: "visitor-1");

        var result = await handler.HandleAsync(command);

        Assert.True(result.IsSuccess);
        Assert.Equal(42, result.PostId);
        Assert.Equal("Dislike", result.ReactionType);
        Assert.Equal(9, result.UserId);
        Assert.Null(result.VisitorIdentifier);
        Assert.True(reactionRepository.CreateWasCalled);
        Assert.Equal(9, reactionRepository.CreatedReaction!.UserId);
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

        public PostReaction? CreatedReaction { get; private set; }

        public Task<PostReaction> CreateAsync(PostReaction reaction, CancellationToken cancellationToken = default)
        {
            CreateWasCalled = true;
            CreatedReaction = reaction;
            return Task.FromResult(reaction);
        }
    }
}
