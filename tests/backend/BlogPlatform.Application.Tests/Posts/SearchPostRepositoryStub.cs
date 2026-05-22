using BlogPlatform.Application.Abstractions;
using BlogPlatform.Domain.Posts;

namespace BlogPlatform.Application.Tests.Posts;

internal sealed class SearchPostRepositoryStub : IPostRepository
{
    private readonly IReadOnlyList<BlogPost> _publicPosts;
    private readonly IReadOnlyList<BlogPost> _searchPosts;

    public SearchPostRepositoryStub(
        IReadOnlyList<BlogPost>? publicPosts = null,
        IReadOnlyList<BlogPost>? searchPosts = null)
    {
        _publicPosts = publicPosts ?? [];
        _searchPosts = searchPosts ?? [];
    }

    public bool ListPublicReadWasCalled { get; private set; }

    public bool SearchPublicReadWasCalled { get; private set; }

    public string? RequestedQuery { get; private set; }

    public int? RequestedUserId { get; private set; }

    public Task<BlogPost> CreateAsync(BlogPost post, CancellationToken cancellationToken = default)
        => throw new NotSupportedException();

    public Task DeleteAsync(int postId, CancellationToken cancellationToken = default)
        => throw new NotSupportedException();

    public Task<BlogPost?> GetByIdAsync(int postId, CancellationToken cancellationToken = default)
        => throw new NotSupportedException();

    public Task<BlogPost?> GetByIdForAuthorAsync(int postId, int authorUserId, CancellationToken cancellationToken = default)
        => throw new NotSupportedException();

    public Task<BlogPost?> GetPublicReadByIdAsync(int postId, CancellationToken cancellationToken = default)
        => throw new NotSupportedException();

    public Task<IReadOnlyList<BlogPost>> ListByAuthorAsync(int authorUserId, CancellationToken cancellationToken = default)
        => throw new NotSupportedException();

    public Task<IReadOnlyList<BlogPost>> ListPublicReadAsync(CancellationToken cancellationToken = default)
    {
        ListPublicReadWasCalled = true;
        return Task.FromResult(_publicPosts);
    }

    public Task<IReadOnlyList<BlogPost>> SearchPublicReadAsync(string query, int? requestingUserId, CancellationToken cancellationToken = default)
    {
        SearchPublicReadWasCalled = true;
        RequestedQuery = query;
        RequestedUserId = requestingUserId;
        return Task.FromResult(_searchPosts);
    }

    public Task<BlogPost> UpdateAsync(BlogPost post, CancellationToken cancellationToken = default)
        => throw new NotSupportedException();
}
