namespace BlogPlatform.Domain.Categories;

public sealed class PostCategory
{
    private PostCategory(int id, string title, string? description, bool isAvailable)
    {
        Id = id;
        Title = title;
        Description = description;
        IsAvailable = isAvailable;
    }

    public int Id { get; }

    public string Title { get; }

    public string? Description { get; }

    public bool IsAvailable { get; }

    public static PostCategory CreateNew(string title, string? description = null)
    {
        var normalizedTitle = NormalizeRequired(title, nameof(title));

        return new PostCategory(
            id: 0,
            title: normalizedTitle,
            description: NormalizeOptional(description),
            isAvailable: true);
    }

    public static PostCategory Rehydrate(int id, string title, string? description = null, bool isAvailable = true)
    {
        if (id <= 0)
        {
            throw new ArgumentException("Category id is required.", nameof(id));
        }

        var normalizedTitle = NormalizeRequired(title, nameof(title));

        return new PostCategory(
            id: id,
            title: normalizedTitle,
            description: NormalizeOptional(description),
            isAvailable: isAvailable);
    }

    public PostCategory UpdateDetails(string title, string? description)
    {
        if (Id <= 0)
        {
            throw new InvalidOperationException("Only persisted categories can be updated.");
        }

        var normalizedTitle = NormalizeRequired(title, nameof(title));

        return new PostCategory(
            id: Id,
            title: normalizedTitle,
            description: NormalizeOptional(description),
            isAvailable: IsAvailable);
    }

    public PostCategory Deactivate()
    {
        if (Id <= 0)
        {
            throw new InvalidOperationException("Only persisted categories can be deactivated.");
        }

        return new PostCategory(
            id: Id,
            title: Title,
            description: Description,
            isAvailable: false);
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
        return string.IsNullOrWhiteSpace(value)
            ? null
            : value.Trim();
    }
}
