namespace BlogPlatform.Api.Contracts.Categories;

public sealed record AdminCategoryListItemResponse(
    int Id,
    string Title,
    string? Description,
    bool IsAvailable);
