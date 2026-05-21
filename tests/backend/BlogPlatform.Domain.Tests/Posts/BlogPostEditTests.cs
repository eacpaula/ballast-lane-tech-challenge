using BlogPlatform.Domain.Posts;

namespace BlogPlatform.Domain.Tests.Posts;

public class BlogPostEditTests
{
    [Fact]
    public void Update_WithValidData_ReturnsUpdatedPost()
    {
        var post = BlogPost.Rehydrate(
            id: 42,
            authorUserId: 7,
            categoryId: 3,
            title: "Original title",
            summary: "Original summary",
            content: "Original content");

        var updatedPost = post.Update(
            title: "  Updated title  ",
            summary: "  Updated summary  ",
            content: "  Updated content  ");

        Assert.Equal(42, updatedPost.Id);
        Assert.Equal(7, updatedPost.AuthorUserId);
        Assert.Equal(3, updatedPost.CategoryId);
        Assert.Equal("Updated title", updatedPost.Title);
        Assert.Equal("Updated summary", updatedPost.Summary);
        Assert.Equal("Updated content", updatedPost.Content);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Update_WithBlankTitle_ThrowsArgumentException(string title)
    {
        var post = BlogPost.Rehydrate(
            id: 42,
            authorUserId: 7,
            categoryId: 3,
            title: "Original title",
            summary: "Original summary",
            content: "Original content");

        void Action() => post.Update(
            title: title,
            summary: "Updated summary",
            content: "Updated content");

        var exception = Assert.Throws<ArgumentException>(Action);

        Assert.Equal("title", exception.ParamName);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Update_WithBlankContent_ThrowsArgumentException(string content)
    {
        var post = BlogPost.Rehydrate(
            id: 42,
            authorUserId: 7,
            categoryId: 3,
            title: "Original title",
            summary: "Original summary",
            content: "Original content");

        void Action() => post.Update(
            title: "Updated title",
            summary: "Updated summary",
            content: content);

        var exception = Assert.Throws<ArgumentException>(Action);

        Assert.Equal("content", exception.ParamName);
    }
}
