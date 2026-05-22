namespace BlogPlatform.Api.Contracts.Categories;

public sealed record UpdateCategoryRequest(string Title, string? Description = null);
