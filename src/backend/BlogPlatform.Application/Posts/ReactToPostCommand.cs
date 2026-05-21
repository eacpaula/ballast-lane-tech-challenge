namespace BlogPlatform.Application.Posts;

public sealed record ReactToPostCommand(
    int PostId,
    string ReactionType,
    int? AuthenticatedUserId,
    string? VisitorIdentifier);
