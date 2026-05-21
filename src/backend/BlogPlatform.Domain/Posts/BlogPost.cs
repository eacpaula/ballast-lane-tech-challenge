namespace BlogPlatform.Domain.Posts;

public sealed class BlogPost
{
    private BlogPost(
        int id,
        int authorUserId,
        int categoryId,
        string title,
        string? summary,
        string content)
    {
        Id = id;
        AuthorUserId = authorUserId;
        CategoryId = categoryId;
        Title = title;
        Summary = summary;
        Content = content;
    }

    public int Id { get; }

    public int AuthorUserId { get; }

    public int CategoryId { get; }

    public string Title { get; }

    public string? Summary { get; }

    public string Content { get; }

    public static BlogPost Create(
        int authorUserId,
        int categoryId,
        string title,
        string? summary,
        string content)
    {
        if (authorUserId <= 0)
        {
            throw new ArgumentException("Author user id is required.", nameof(authorUserId));
        }

        if (categoryId <= 0)
        {
            throw new ArgumentException("Category id is required.", nameof(categoryId));
        }

        var normalizedTitle = NormalizeRequired(title, nameof(title));
        var normalizedContent = NormalizeRequired(content, nameof(content));
        var normalizedSummary = NormalizeOptional(summary);

        return new BlogPost(
            id: 0,
            authorUserId: authorUserId,
            categoryId: categoryId,
            title: normalizedTitle,
            summary: normalizedSummary,
            content: normalizedContent);
    }

    public static BlogPost Rehydrate(
        int id,
        int authorUserId,
        int categoryId,
        string title,
        string? summary,
        string content)
    {
        if (id <= 0)
        {
            throw new ArgumentException("Post id is required.", nameof(id));
        }

        var normalizedTitle = NormalizeRequired(title, nameof(title));
        var normalizedContent = NormalizeRequired(content, nameof(content));
        var normalizedSummary = NormalizeOptional(summary);

        return new BlogPost(
            id: id,
            authorUserId: authorUserId,
            categoryId: categoryId,
            title: normalizedTitle,
            summary: normalizedSummary,
            content: normalizedContent);
    }

    public BlogPost Update(
        string title,
        string? summary,
        string content)
    {
        if (Id <= 0)
        {
            throw new InvalidOperationException("Only persisted posts can be updated.");
        }

        var normalizedTitle = NormalizeRequired(title, nameof(title));
        var normalizedContent = NormalizeRequired(content, nameof(content));
        var normalizedSummary = NormalizeOptional(summary);

        return new BlogPost(
            id: Id,
            authorUserId: AuthorUserId,
            categoryId: CategoryId,
            title: normalizedTitle,
            summary: normalizedSummary,
            content: normalizedContent);
    }

    private static string NormalizeRequired(string value, string paramName)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new ArgumentException($"{paramName} is required.", paramName);
        }

        return value.Trim();
    }

    private static string? NormalizeOptional(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return null;
        }

        return value.Trim();
    }
}
