namespace BlogPlatform.Domain.Reactions;

public sealed class ReactionType
{
    private ReactionType(string value)
    {
        Value = value;
    }

    public string Value { get; }

    public static ReactionType Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new ArgumentException("reactionType is required.", nameof(value));
        }

        var normalizedValue = value.Trim().ToLowerInvariant();

        return normalizedValue switch
        {
            "like" => new ReactionType("Like"),
            "dislike" => new ReactionType("Dislike"),
            _ => throw new ArgumentException("reactionType is invalid.", nameof(value)),
        };
    }
}
