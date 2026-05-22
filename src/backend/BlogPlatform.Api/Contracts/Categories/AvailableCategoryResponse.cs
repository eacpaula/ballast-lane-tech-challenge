namespace BlogPlatform.Api.Contracts.Categories;

public sealed record AvailableCategoryResponse(
    int Id,
    string Title,
    string? Description,
    bool IsAvailable);
