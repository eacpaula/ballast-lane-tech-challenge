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
        bool isAvailable,
        IReadOnlyList<string> tags,
        DateTimeOffset? publishDate = null,
        DateTimeOffset? expirationDate = null)
    {
        PostId = postId;
        CategoryId = categoryId;
        Title = title;
        Summary = summary;
        IsPublic = isPublic;
        IsAvailable = isAvailable;
        Tags = tags;
        PublishDate = publishDate;
        ExpirationDate = expirationDate;
    }

    public int PostId { get; }
    public int CategoryId { get; }
    public string Title { get; }
    public string? Summary { get; }
    public bool IsPublic { get; }
    public bool IsAvailable { get; }
    public IReadOnlyList<string> Tags { get; }
    public DateTimeOffset? PublishDate { get; }
    public DateTimeOffset? ExpirationDate { get; }

    public static OwnedPostListItem From(BlogPost post)
    {
        ArgumentNullException.ThrowIfNull(post);
        return new OwnedPostListItem(
            post.Id,
            post.CategoryId,
            post.Title,
            post.Summary,
            post.IsPublic,
            post.IsAvailable,
            post.Tags,
            post.PublishDate,
            post.ExpirationDate);
    }
}
