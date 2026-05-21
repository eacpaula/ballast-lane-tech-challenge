namespace BlogPlatform.Api.Contracts.Reactions;

public sealed record ReactionResponse(
    int PostId,
    string ReactionType,
    int? UserId,
    string? VisitorIdentifier);
