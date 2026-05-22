namespace BlogPlatform.Application.Posts;

public static class TagNormalizer
{
    public static IReadOnlyList<string> Normalize(IEnumerable<string>? tags)
    {
        if (tags is null)
        {
            return Array.Empty<string>();
        }

        var seen = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        var result = new List<string>();

        foreach (var tag in tags)
        {
            var trimmed = tag.Trim();

            if (trimmed.Length == 0)
            {
                continue;
            }

            if (seen.Add(trimmed))
            {
                result.Add(trimmed);
            }
        }

        return result;
    }
}
