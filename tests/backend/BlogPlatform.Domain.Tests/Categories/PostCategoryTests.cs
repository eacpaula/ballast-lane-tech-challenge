using BlogPlatform.Domain.Categories;

namespace BlogPlatform.Domain.Tests.Categories;

public class PostCategoryTests
{
    [Fact]
    public void CreateNew_WithValidTitle_ReturnsNormalizedCategory()
    {
        var category = PostCategory.CreateNew("  Architecture  ", "  Posts about architecture.  ");

        Assert.Equal(0, category.Id);
        Assert.Equal("Architecture", category.Title);
        Assert.Equal("Posts about architecture.", category.Description);
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
    public void UpdateDetails_WithValidTitle_ReturnsUpdatedCategory()
    {
        var category = PostCategory.Rehydrate(
            id: 8,
            title: "Backend",
            description: "Original description",
            isAvailable: true);

        var updatedCategory = category.UpdateDetails("  Testing  ", "  Updated description  ");

        Assert.Equal(8, updatedCategory.Id);
        Assert.Equal("Testing", updatedCategory.Title);
        Assert.Equal("Updated description", updatedCategory.Description);
        Assert.True(updatedCategory.IsAvailable);
    }

    [Fact]
    public void UpdateDetails_WithBlankDescription_ClearsDescription()
    {
        var category = PostCategory.Rehydrate(
            id: 8,
            title: "Backend",
            description: "Original description",
            isAvailable: true);

        var updatedCategory = category.UpdateDetails("Backend", "   ");

        Assert.Null(updatedCategory.Description);
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
        Assert.Null(deactivatedCategory.Description);
        Assert.False(deactivatedCategory.IsAvailable);
    }
}
