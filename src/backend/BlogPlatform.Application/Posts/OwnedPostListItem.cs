using BlogPlatform.Domain.Posts;

namespace BlogPlatform.Application.Posts;

public sealed class OwnedPostListItem
{
    public OwnedPostListItem(
        int postId,
        int categoryId,
        string title,
        string? summary,
        bool isPublic,
        bool isAvailable)
    {
        PostId = postId;
        CategoryId = categoryId;
        Title = title;
        Summary = summary;
        IsPublic = isPublic;
        IsAvailable = isAvailable;
    }

    public int PostId { get; }
    public int CategoryId { get; }
    public string Title { get; }
    public string? Summary { get; }
    public bool IsPublic { get; }
    public bool IsAvailable { get; }

    public static OwnedPostListItem From(BlogPost post)
    {
        ArgumentNullException.ThrowIfNull(post);
        return new OwnedPostListItem(post.Id, post.CategoryId, post.Title, post.Summary, post.IsPublic, post.IsAvailable);
    }
}
