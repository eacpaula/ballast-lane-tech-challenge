namespace BlogPlatform.Domain.Reactions;

public sealed class ReactionActor
{
    private ReactionActor(int? userId, string? visitorIdentifier)
    {
        UserId = userId;
        VisitorIdentifier = visitorIdentifier;
    }

    public int? UserId { get; }

    public string? VisitorIdentifier { get; }

    public static ReactionActor Create(int? userId, string? visitorIdentifier)
    {
        if (userId.HasValue && userId.Value > 0)
        {
            return new ReactionActor(userId.Value, null);
        }

        if (!string.IsNullOrWhiteSpace(visitorIdentifier))
        {
            return new ReactionActor(null, visitorIdentifier.Trim());
        }

        throw new ArgumentException(
            "A valid userId or visitorIdentifier is required.",
            nameof(visitorIdentifier));
    }
}
