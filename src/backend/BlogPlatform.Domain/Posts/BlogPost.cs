namespace BlogPlatform.Domain.Posts;

public sealed class BlogPost
{
    private BlogPost(
        int id,
        int authorUserId,
        int categoryId,
        string title,
        string? summary,
        string content,
        bool isPublic,
        bool isAvailable,
        IReadOnlyList<string> tags,
        DateTimeOffset? publishDate,
        DateTimeOffset? expirationDate)
    {
        Id = id;
        AuthorUserId = authorUserId;
        CategoryId = categoryId;
        Title = title;
        Summary = summary;
        Content = content;
        IsPublic = isPublic;
        IsAvailable = isAvailable;
        Tags = tags;
        PublishDate = publishDate;
        ExpirationDate = expirationDate;
    }

    public int Id { get; }

    public int AuthorUserId { get; }

    public int CategoryId { get; }

    public string Title { get; }

    public string? Summary { get; }

    public string Content { get; }

    public bool IsPublic { get; }

    public bool IsAvailable { get; }

    public DateTimeOffset? PublishDate { get; }

    public DateTimeOffset? ExpirationDate { get; }

    public bool IsPubliclyReadable =>
        IsPublic &&
        IsAvailable &&
        (PublishDate is null || PublishDate <= DateTimeOffset.UtcNow) &&
        (ExpirationDate is null || ExpirationDate > DateTimeOffset.UtcNow);

    public IReadOnlyList<string> Tags { get; }

    public static BlogPost Create(
        int authorUserId,
        int categoryId,
        string title,
        string? summary,
        string content,
        bool isPublic = true,
        bool isAvailable = true,
        IEnumerable<string>? tags = null,
        DateTimeOffset? publishDate = null,
        DateTimeOffset? expirationDate = null)
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
            content: normalizedContent,
            isPublic: isPublic,
            isAvailable: isAvailable,
            tags: tags?.ToList() ?? (IReadOnlyList<string>)Array.Empty<string>(),
            publishDate: publishDate,
            expirationDate: expirationDate);
    }

    public static BlogPost Rehydrate(
        int id,
        int authorUserId,
        int categoryId,
        string title,
        string? summary,
        string content,
        bool isPublic = true,
        bool isAvailable = true,
        IEnumerable<string>? tags = null,
        DateTimeOffset? publishDate = null,
        DateTimeOffset? expirationDate = null)
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
            content: normalizedContent,
            isPublic: isPublic,
            isAvailable: isAvailable,
            tags: tags?.ToList() ?? (IReadOnlyList<string>)Array.Empty<string>(),
            publishDate: publishDate,
            expirationDate: expirationDate);
    }

    public BlogPost Update(
        string title,
        string? summary,
        string content,
        IEnumerable<string>? tags = null,
        DateTimeOffset? publishDate = null,
        DateTimeOffset? expirationDate = null)
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
            content: normalizedContent,
            isPublic: IsPublic,
            isAvailable: IsAvailable,
            tags: tags?.ToList() ?? (IReadOnlyList<string>)Array.Empty<string>(),
            publishDate: publishDate,
            expirationDate: expirationDate);
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
