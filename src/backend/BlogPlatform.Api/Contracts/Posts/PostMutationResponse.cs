namespace BlogPlatform.Api.Contracts.Posts;

public sealed record PostMutationResponse(
    int Id,
    int AuthorUserId,
    string Title,
    string? Summary,
    string? Content,
    IReadOnlyList<string> Tags,
    DateTimeOffset? PublishDate = null,
    DateTimeOffset? ExpirationDate = null);
