using BlogPlatform.Application.Abstractions;
using BlogPlatform.Application.Posts;
using BlogPlatform.Domain.Posts;

namespace BlogPlatform.Application.Tests.Posts;

internal sealed class PaginatedPostRepositoryStub : IPostRepository
{
    private readonly PaginatedBlogPostReadResult _result;

    public PaginatedPostRepositoryStub(PaginatedBlogPostReadResult? result = null)
    {
        _result = result ?? new PaginatedBlogPostReadResult([], 0);
    }

    public PostListPageRequest? RequestedPage { get; private set; }

    public int Calls { get; private set; }

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
        => throw new NotSupportedException();

    public Task<PaginatedBlogPostReadResult> ListPublicReadPageAsync(
        PostListPageRequest request,
        CancellationToken cancellationToken = default)
    {
        RequestedPage = request;
        Calls++;
        return Task.FromResult(_result);
    }

    public Task<IReadOnlyList<BlogPost>> SearchPublicReadAsync(
        string query,
        int? requestingUserId,
        CancellationToken cancellationToken = default)
        => throw new NotSupportedException();

    public Task<BlogPost> UpdateAsync(BlogPost post, CancellationToken cancellationToken = default)
        => throw new NotSupportedException();
}
