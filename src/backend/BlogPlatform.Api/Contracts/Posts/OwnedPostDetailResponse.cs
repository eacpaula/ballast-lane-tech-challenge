namespace BlogPlatform.Api.Contracts.Posts;

public sealed record OwnedPostDetailResponse(
    int Id,
    int AuthorUserId,
    int CategoryId,
    string Title,
    string? Summary,
    string Content,
    bool IsPublic,
    bool IsAvailable,
    IReadOnlyList<string> Tags);
