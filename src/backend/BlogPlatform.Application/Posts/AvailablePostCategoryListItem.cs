using BlogPlatform.Domain.Categories;

namespace BlogPlatform.Application.Posts;

public sealed class AvailablePostCategoryListItem
{
    public AvailablePostCategoryListItem(int categoryId, string title)
    {
        CategoryId = categoryId;
        Title = title;
    }

    public int CategoryId { get; }

    public string Title { get; }

    public static AvailablePostCategoryListItem From(PostCategory category)
    {
        ArgumentNullException.ThrowIfNull(category);
        return new AvailablePostCategoryListItem(category.Id, category.Title);
    }
}
