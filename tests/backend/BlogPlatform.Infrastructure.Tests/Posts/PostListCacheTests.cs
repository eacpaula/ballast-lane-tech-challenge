using BlogPlatform.Application.Posts;
using BlogPlatform.Infrastructure.Caching;

namespace BlogPlatform.Infrastructure.Tests.Posts;

[Collection("PostgreSqlIntegration")]
public sealed class PostListCacheTests : IClassFixture<RedisIntegrationTestCache>
{
    private readonly RedisIntegrationTestCache _cache;

    public PostListCacheTests(RedisIntegrationTestCache cache) => _cache = cache;

    [Fact]
    public async Task RedisPostListCache_RoundTripsPaginatedResults()
    {
        await _cache.VerifyConnectivityAsync();
        await _cache.ResetAsync();
        var provider = new RedisPostListCache(new RedisConnectionFactory(_cache.CreateSettings()));
        var expected = new PaginatedPublicPostResult(
        [
            new PublicPostListItem(7, "Cached architecture", "Cached summary", []),
        ],
        page: 1,
        pageSize: 6,
        totalCount: 1);

        await provider.SetAsync("posts:list|query:architecture|page:1|size:6|viewer:anonymous", expected, TimeSpan.FromSeconds(30));

        var actual = await provider.GetAsync("posts:list|query:architecture|page:1|size:6|viewer:anonymous");

        Assert.NotNull(actual);
        Assert.Single(actual!.Items);
        Assert.Equal(7, actual.Items[0].PostId);
        Assert.Equal(1, actual.TotalCount);
    }

    [Fact]
    public async Task RedisPostListCache_RespectsConfiguredTtl()
    {
        await _cache.VerifyConnectivityAsync();
        await _cache.ResetAsync();
        var provider = new RedisPostListCache(new RedisConnectionFactory(_cache.CreateSettings()));
        var value = new PaginatedPublicPostResult([], page: 1, pageSize: 6, totalCount: 0);

        await provider.SetAsync("posts:list|query:ttl|page:1|size:6|viewer:user:2", value, TimeSpan.FromSeconds(1));
        await Task.Delay(TimeSpan.FromSeconds(2));

        var actual = await provider.GetAsync("posts:list|query:ttl|page:1|size:6|viewer:user:2");

        Assert.Null(actual);
    }
}
