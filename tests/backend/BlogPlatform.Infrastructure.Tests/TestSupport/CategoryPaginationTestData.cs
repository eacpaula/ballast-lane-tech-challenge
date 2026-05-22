namespace BlogPlatform.Infrastructure.Tests.TestSupport;

internal static class CategoryPaginationTestData
{
    public static void AssertValidPage<TItem>(BlogPlatform.Application.Posts.PaginatedCategoryResult<TItem> result, int page, int pageSize)
    {
        Assert.Equal(page, result.Page);
        Assert.Equal(pageSize, result.PageSize);
        Assert.True(result.TotalCount >= 0);
        Assert.True(result.TotalPages >= 0);
    }
}
