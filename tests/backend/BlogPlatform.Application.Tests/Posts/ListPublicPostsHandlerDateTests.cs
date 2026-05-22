using BlogPlatform.Application.Abstractions;
using BlogPlatform.Application.Posts;
using BlogPlatform.Domain.Posts;

namespace BlogPlatform.Application.Tests.Posts;

public class ListPublicPostsHandlerDateTests
{
    [Fact]
    public async Task HandleAsync_ExcludesPostsWithFuturePublishDate()
    {
        var posts = new[]
        {
            BlogPost.Rehydrate(1, 1, 1, "Active", null, "content", isPublic: true, isAvailable: true),
            BlogPost.Rehydrate(2, 1, 1, "Scheduled", null, "content", isPublic: true, isAvailable: true,
                publishDate: DateTimeOffset.UtcNow.AddDays(7)),
        };

        var result = await new ListPublicPostsHandler(new StubPostRepository(posts)).HandleAsync();

        Assert.DoesNotContain(result, p => p.PostId == 2);
        Assert.Contains(result, p => p.PostId == 1);
    }

    [Fact]
    public async Task HandleAsync_ExcludesPostsWithPastExpirationDate()
    {
        var posts = new[]
        {
            BlogPost.Rehydrate(1, 1, 1, "Active", null, "content", isPublic: true, isAvailable: true),
            BlogPost.Rehydrate(3, 1, 1, "Expired", null, "content", isPublic: true, isAvailable: true,
                expirationDate: DateTimeOffset.UtcNow.AddDays(-1)),
        };

        var result = await new ListPublicPostsHandler(new StubPostRepository(posts)).HandleAsync();

        Assert.DoesNotContain(result, p => p.PostId == 3);
        Assert.Contains(result, p => p.PostId == 1);
    }

    [Fact]
    public async Task HandleAsync_IncludesPostsWithNoDates()
    {
        var posts = new[]
        {
            BlogPost.Rehydrate(4, 1, 1, "No dates", null, "content", isPublic: true, isAvailable: true),
        };

        var result = await new ListPublicPostsHandler(new StubPostRepository(posts)).HandleAsync();

        Assert.Single(result);
        Assert.Equal(4, result[0].PostId);
    }

    [Fact]
    public async Task HandleAsync_IncludesPostsWithActiveWindow()
    {
        var posts = new[]
        {
            BlogPost.Rehydrate(5, 1, 1, "In window", null, "content", isPublic: true, isAvailable: true,
                publishDate: DateTimeOffset.UtcNow.AddDays(-1),
                expirationDate: DateTimeOffset.UtcNow.AddDays(7)),
        };

        var result = await new ListPublicPostsHandler(new StubPostRepository(posts)).HandleAsync();

        Assert.Single(result);
        Assert.Equal(5, result[0].PostId);
    }

    private sealed class StubPostRepository(IReadOnlyList<BlogPost> posts) : IPostRepository
    {
        public Task<IReadOnlyList<BlogPost>> ListPublicReadAsync(CancellationToken cancellationToken = default)
            => Task.FromResult(posts);

        public Task<BlogPost> CreateAsync(BlogPost post, CancellationToken cancellationToken = default) => throw new NotSupportedException();
        public Task DeleteAsync(int postId, CancellationToken cancellationToken = default) => throw new NotSupportedException();
        public Task<BlogPost?> GetByIdAsync(int postId, CancellationToken cancellationToken = default) => throw new NotSupportedException();
        public Task<BlogPost?> GetByIdForAuthorAsync(int postId, int authorUserId, CancellationToken cancellationToken = default) => throw new NotSupportedException();
        public Task<BlogPost?> GetPublicReadByIdAsync(int postId, CancellationToken cancellationToken = default) => throw new NotSupportedException();
        public Task<IReadOnlyList<BlogPost>> ListByAuthorAsync(int authorUserId, CancellationToken cancellationToken = default) => throw new NotSupportedException();
        public Task<IReadOnlyList<BlogPost>> SearchPublicReadAsync(string query, int? requestingUserId, CancellationToken cancellationToken = default) => throw new NotSupportedException();
        public Task<BlogPost> UpdateAsync(BlogPost post, CancellationToken cancellationToken = default) => throw new NotSupportedException();
    }
}
