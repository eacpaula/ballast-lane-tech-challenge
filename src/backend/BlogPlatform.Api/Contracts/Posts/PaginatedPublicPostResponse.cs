namespace BlogPlatform.Api.Contracts.Posts;

public sealed record PaginatedPublicPostResponse(
    IReadOnlyList<PublicPostSummaryResponse> Items,
    int Page,
    int PageSize,
    int TotalCount,
    int TotalPages,
    bool HasNextPage);
