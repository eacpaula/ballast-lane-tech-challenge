namespace BlogPlatform.Api.Contracts.Posts;

public sealed record UpdatePostRequest(
    string Title,
    string? Summary,
    string Content,
    IReadOnlyList<string>? Tags = null);
