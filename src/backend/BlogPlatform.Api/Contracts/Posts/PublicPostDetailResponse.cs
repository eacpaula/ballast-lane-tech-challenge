namespace BlogPlatform.Api.Contracts.Posts;

public sealed record PublicPostDetailResponse(
    int Id,
    string Title,
    string? Summary,
    string Content,
    IReadOnlyList<string> Tags,
    DateTimeOffset? PublishDate = null,
    DateTimeOffset? ExpirationDate = null);
