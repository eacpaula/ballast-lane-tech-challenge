namespace BlogPlatform.Api.Contracts.Posts;

public sealed record PublicPostSummaryResponse(
    int Id,
    string Title,
    string? Summary,
    IReadOnlyList<string> Tags,
    DateTimeOffset? PublishDate = null,
    DateTimeOffset? ExpirationDate = null);
