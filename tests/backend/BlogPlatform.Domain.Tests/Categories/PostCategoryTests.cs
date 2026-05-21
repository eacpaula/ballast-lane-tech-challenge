using BlogPlatform.Domain.Categories;

namespace BlogPlatform.Domain.Tests.Categories;

public class PostCategoryTests
{
    [Fact]
    public void CreateNew_WithValidTitle_ReturnsNormalizedCategory()
    {
        var category = PostCategory.CreateNew("  Architecture  ");

        Assert.Equal(0, category.Id);
        Assert.Equal("Architecture", category.Title);
        Assert.True(category.IsAvailable);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void CreateNew_WithBlankTitle_ThrowsArgumentException(string title)
    {
        var action = () => PostCategory.CreateNew(title);

        var exception = Assert.Throws<ArgumentException>(action);

        Assert.Equal("title", exception.ParamName);
    }

    [Fact]
    public void UpdateTitle_WithValidTitle_ReturnsUpdatedCategory()
    {
        var category = PostCategory.Rehydrate(
            id: 8,
            title: "Backend",
            isAvailable: true);

        var updatedCategory = category.UpdateTitle("  Testing  ");

        Assert.Equal(8, updatedCategory.Id);
        Assert.Equal("Testing", updatedCategory.Title);
        Assert.True(updatedCategory.IsAvailable);
    }

    [Fact]
    public void Deactivate_ReturnsUnavailableCategory()
    {
        var category = PostCategory.Rehydrate(
            id: 8,
            title: "Backend",
            isAvailable: true);

        var deactivatedCategory = category.Deactivate();

        Assert.Equal(8, deactivatedCategory.Id);
        Assert.Equal("Backend", deactivatedCategory.Title);
        Assert.False(deactivatedCategory.IsAvailable);
    }
}
