using BlogPlatform.Domain.Categories;

namespace BlogPlatform.Application.Posts;

public sealed class AvailablePostCategoryListItem
{
    public AvailablePostCategoryListItem(int categoryId, string title, string? description, bool isAvailable)
    {
        CategoryId = categoryId;
        Title = title;
        Description = description;
        IsAvailable = isAvailable;
    }

    public int CategoryId { get; }

    public string Title { get; }

    public string? Description { get; }

    public bool IsAvailable { get; }

    public static AvailablePostCategoryListItem From(PostCategory category)
    {
        ArgumentNullException.ThrowIfNull(category);
        return new AvailablePostCategoryListItem(category.Id, category.Title, category.Description, category.IsAvailable);
    }
}
