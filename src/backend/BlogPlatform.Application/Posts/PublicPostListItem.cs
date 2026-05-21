using BlogPlatform.Domain.Posts;

namespace BlogPlatform.Application.Posts;

public sealed class PublicPostListItem
{
    public PublicPostListItem(int postId, string title, string? summary)
    {
        PostId = postId;
        Title = title;
        Summary = summary;
    }

    public int PostId { get; }

    public string Title { get; }

    public string? Summary { get; }

    public static PublicPostListItem From(BlogPost post)
    {
        return new PublicPostListItem(
            postId: post.Id,
            title: post.Title,
            summary: post.Summary);
    }
}
