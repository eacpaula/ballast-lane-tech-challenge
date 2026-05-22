namespace BlogPlatform.Api.Contracts.Categories;

public sealed record CategoryResponse(
    int Id,
    string Title,
    string? Description,
    bool IsAvailable);
