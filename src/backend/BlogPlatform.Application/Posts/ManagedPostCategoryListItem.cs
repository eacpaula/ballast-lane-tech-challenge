using BlogPlatform.Domain.Categories;

namespace BlogPlatform.Application.Posts;

public sealed class ManagedPostCategoryListItem
{
    public ManagedPostCategoryListItem(int categoryId, string title, string? description, bool isAvailable)
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

    public static ManagedPostCategoryListItem From(PostCategory category)
    {
        ArgumentNullException.ThrowIfNull(category);
        return new ManagedPostCategoryListItem(category.Id, category.Title, category.Description, category.IsAvailable);
    }
}
