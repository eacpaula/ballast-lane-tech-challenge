namespace BlogPlatform.Api.Contracts.Posts;

public sealed record CreatePostRequest(
    int CategoryId,
    string Title,
    string? Summary,
    string Content,
    IReadOnlyList<string>? Tags = null,
    DateTimeOffset? PublishDate = null,
    DateTimeOffset? ExpirationDate = null);
