namespace BlogPlatform.Application.Posts;

public static class PostListCacheKeyFactory
{
    public static string Create(PostListPageRequest request)
    {
        ArgumentNullException.ThrowIfNull(request);

        var queryMarker = request.Query is null
            ? "query:__all__"
            : $"query:{request.Query.Trim().ToLowerInvariant()}";
        var viewerMarker = request.RequestingUserId.HasValue
            ? $"viewer:user:{request.RequestingUserId.Value}"
            : "viewer:anonymous";

        return $"posts:list|{queryMarker}|page:{request.Page}|size:{request.PageSize}|{viewerMarker}";
    }
}
