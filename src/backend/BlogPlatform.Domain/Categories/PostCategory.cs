namespace BlogPlatform.Domain.Categories;

public sealed class PostCategory
{
    private PostCategory(int id, string title, bool isAvailable)
    {
        Id = id;
        Title = title;
        IsAvailable = isAvailable;
    }

    public int Id { get; }

    public string Title { get; }

    public bool IsAvailable { get; }

    public static PostCategory CreateNew(string title)
    {
        var normalizedTitle = NormalizeRequired(title, nameof(title));

        return new PostCategory(
            id: 0,
            title: normalizedTitle,
            isAvailable: true);
    }

    public static PostCategory Rehydrate(int id, string title, bool isAvailable = true)
    {
        if (id <= 0)
        {
            throw new ArgumentException("Category id is required.", nameof(id));
        }

        var normalizedTitle = NormalizeRequired(title, nameof(title));

        return new PostCategory(
            id: id,
            title: normalizedTitle,
            isAvailable: isAvailable);
    }

    public PostCategory UpdateTitle(string title)
    {
        if (Id <= 0)
        {
            throw new InvalidOperationException("Only persisted categories can be updated.");
        }

        var normalizedTitle = NormalizeRequired(title, nameof(title));

        return new PostCategory(
            id: Id,
            title: normalizedTitle,
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
}
