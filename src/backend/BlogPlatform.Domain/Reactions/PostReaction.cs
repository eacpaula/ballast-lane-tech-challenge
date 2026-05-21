namespace BlogPlatform.Domain.Reactions;

public sealed class PostReaction
{
    private PostReaction(int postId, ReactionType reactionType, ReactionActor actor)
    {
        PostId = postId;
        ReactionType = reactionType;
        UserId = actor.UserId;
        VisitorIdentifier = actor.VisitorIdentifier;
    }

    public int PostId { get; }

    public ReactionType ReactionType { get; }

    public int? UserId { get; }

    public string? VisitorIdentifier { get; }

    public static PostReaction Create(int postId, ReactionType reactionType, ReactionActor actor)
    {
        if (postId <= 0)
        {
            throw new ArgumentException("postId is required.", nameof(postId));
        }

        ArgumentNullException.ThrowIfNull(reactionType);
        ArgumentNullException.ThrowIfNull(actor);

        return new PostReaction(postId, reactionType, actor);
    }
}
