namespace BlogPlatform.Api.Contracts.Posts;

public sealed record OwnedPostSummaryResponse(
    int Id,
    int CategoryId,
    string Title,
    string? Summary,
    bool IsPublic,
    bool IsAvailable,
    IReadOnlyList<string> Tags);
