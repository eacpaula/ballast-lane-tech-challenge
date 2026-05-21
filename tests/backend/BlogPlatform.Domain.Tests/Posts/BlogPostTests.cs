using BlogPlatform.Domain.Posts;

namespace BlogPlatform.Domain.Tests.Posts;

public class BlogPostTests
{
    [Fact]
    public void Create_WithValidData_ReturnsPost()
    {
        var post = BlogPost.Create(
            authorUserId: 10,
            categoryId: 5,
            title: "  Clean Architecture  ",
            summary: "  Summary  ",
            content: "  Useful content.  ");

        Assert.Equal(10, post.AuthorUserId);
        Assert.Equal(5, post.CategoryId);
        Assert.Equal("Clean Architecture", post.Title);
        Assert.Equal("Summary", post.Summary);
        Assert.Equal("Useful content.", post.Content);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Create_WithBlankTitle_ThrowsArgumentException(string title)
    {
        var action = () => BlogPost.Create(
            authorUserId: 10,
            categoryId: 5,
            title: title,
            summary: "Summary",
            content: "Useful content.");

        var exception = Assert.Throws<ArgumentException>(action);

        Assert.Equal("title", exception.ParamName);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Create_WithBlankContent_ThrowsArgumentException(string content)
    {
        var action = () => BlogPost.Create(
            authorUserId: 10,
            categoryId: 5,
            title: "Valid title",
            summary: "Summary",
            content: content);

        var exception = Assert.Throws<ArgumentException>(action);

        Assert.Equal("content", exception.ParamName);
    }
}
