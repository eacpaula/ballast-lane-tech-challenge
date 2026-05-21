using BlogPlatform.Application.Abstractions;
using BlogPlatform.Domain.Reactions;

namespace BlogPlatform.Application.Posts;

public sealed class ReactToPostHandler
{
    private readonly IPostRepository _postRepository;
    private readonly IPostReactionRepository _postReactionRepository;

    public ReactToPostHandler(
        IPostRepository postRepository,
        IPostReactionRepository postReactionRepository)
    {
        _postRepository = postRepository ?? throw new ArgumentNullException(nameof(postRepository));
        _postReactionRepository = postReactionRepository ?? throw new ArgumentNullException(nameof(postReactionRepository));
    }

    public async Task<ReactToPostResult> HandleAsync(
        ReactToPostCommand command,
        CancellationToken cancellationToken = default)
    {
        ReactionType reactionType;
        ReactionActor actor;

        try
        {
            reactionType = ReactionType.Create(command.ReactionType);
            actor = ReactionActor.Create(command.AuthenticatedUserId, command.VisitorIdentifier);
        }
        catch (ArgumentException exception) when (command.ReactionType is null || exception.Message.Contains("reactionType", StringComparison.Ordinal))
        {
            return ReactToPostResult.Failure(
                errorCode: "InvalidReactionType",
                errorMessage: "The submitted reaction type is not supported.");
        }
        catch (ArgumentException)
        {
            return ReactToPostResult.Failure(
                errorCode: "InvalidReactionActor",
                errorMessage: "A valid user or visitor identity is required to react to a post.");
        }

        var post = await _postRepository.GetByIdAsync(command.PostId, cancellationToken);

        if (post is null || !post.IsPubliclyReadable)
        {
            return ReactToPostResult.Failure(
                errorCode: "PostNotAvailable",
                errorMessage: "The requested post is not available for public reactions.");
        }

        var reaction = PostReaction.Create(post.Id, reactionType, actor);
        var savedReaction = await _postReactionRepository.CreateAsync(reaction, cancellationToken);

        return ReactToPostResult.Success(savedReaction);
    }
}
