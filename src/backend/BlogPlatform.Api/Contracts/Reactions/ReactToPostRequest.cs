namespace BlogPlatform.Api.Contracts.Reactions;

public sealed record ReactToPostRequest(
    string ReactionType,
    string? VisitorIdentifier);
