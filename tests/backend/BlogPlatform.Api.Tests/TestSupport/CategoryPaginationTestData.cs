using BlogPlatform.Api.Contracts.Categories;

namespace BlogPlatform.Api.Tests.TestSupport;

internal static class CategoryPaginationTestData
{
    public static void AssertValidPage<TItem>(PaginatedCategoryResponse<TItem>? response, int page, int pageSize)
    {
        Assert.NotNull(response);
        Assert.Equal(page, response!.Page);
        Assert.Equal(pageSize, response.PageSize);
        Assert.True(response.TotalCount >= 0);
        Assert.True(response.TotalPages >= 0);
    }
}
