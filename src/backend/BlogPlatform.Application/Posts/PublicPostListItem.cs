using BlogPlatform.Domain.Posts;

namespace BlogPlatform.Application.Posts;

public sealed class PublicPostListItem
{
    public PublicPostListItem(int postId, string title, string? summary, IReadOnlyList<string> tags)
    {
        PostId = postId;
        Title = title;
        Summary = summary;
        Tags = tags;
    }

    public int PostId { get; }

    public string Title { get; }

    public string? Summary { get; }

    public IReadOnlyList<string> Tags { get; }

    public static PublicPostListItem From(BlogPost post)
    {
        return new PublicPostListItem(
            postId: post.Id,
            title: post.Title,
            summary: post.Summary,
            tags: post.Tags);
    }
}
