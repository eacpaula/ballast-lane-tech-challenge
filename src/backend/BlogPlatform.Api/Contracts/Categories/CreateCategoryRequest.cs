namespace BlogPlatform.Api.Contracts.Categories;

public sealed record CreateCategoryRequest(string Title, string? Description = null);
