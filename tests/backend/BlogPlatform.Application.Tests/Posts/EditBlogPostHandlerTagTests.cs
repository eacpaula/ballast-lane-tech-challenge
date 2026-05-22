using BlogPlatform.Application.Abstractions;
using BlogPlatform.Application.Posts;
using BlogPlatform.Domain.Posts;

namespace BlogPlatform.Application.Tests.Posts;

public class EditBlogPostHandlerTagTests
{
    [Fact]
    public async Task HandleAsync_WithNewTags_ReplacesExistingTagsOnResult()
    {
        var existingPost = BlogPost.Rehydrate(
            id: 42,
            authorUserId: 7,
            categoryId: 3,
            title: "Title",
            summary: null,
            content: "Content",
            tags: new[] { "old-tag" });

        var postRepository = new FakePostRepository(existingPost);
        var handler = new EditBlogPostHandler(postRepository);

        var command = new EditBlogPostCommand(
            AuthenticatedUserId: 7,
            PostId: 42,
            Title: "Title",
            Summary: null,
            Content: "Content",
            Tags: new[] { "  new-tag  ", "another" });

        var result = await handler.HandleAsync(command);

        Assert.True(result.IsSuccess);
        Assert.Equal(2, result.Tags.Count);
        Assert.Equal("new-tag", result.Tags[0]);
        Assert.Equal("another", result.Tags[1]);
    }

    [Fact]
    public async Task HandleAsync_WithEmptyTags_ClearsAllTags()
    {
        var existingPost = BlogPost.Rehydrate(
            id: 42,
            authorUserId: 7,
            categoryId: 3,
            title: "Title",
            summary: null,
            content: "Content",
            tags: new[] { "old-tag" });

        var postRepository = new FakePostRepository(existingPost);
        var handler = new EditBlogPostHandler(postRepository);

        var command = new EditBlogPostCommand(
            AuthenticatedUserId: 7,
            PostId: 42,
            Title: "Title",
            Summary: null,
            Content: "Content",
            Tags: Array.Empty<string>());

        var result = await handler.HandleAsync(command);

        Assert.True(result.IsSuccess);
        Assert.Empty(result.Tags);
    }

    [Fact]
    public async Task HandleAsync_WithNullTags_ClearsAllTags()
    {
        var existingPost = BlogPost.Rehydrate(
            id: 42,
            authorUserId: 7,
            categoryId: 3,
            title: "Title",
            summary: null,
            content: "Content",
            tags: new[] { "old-tag" });

        var postRepository = new FakePostRepository(existingPost);
        var handler = new EditBlogPostHandler(postRepository);

        var command = new EditBlogPostCommand(
            AuthenticatedUserId: 7,
            PostId: 42,
            Title: "Title",
            Summary: null,
            Content: "Content",
            Tags: null);

        var result = await handler.HandleAsync(command);

        Assert.True(result.IsSuccess);
        Assert.Empty(result.Tags);
    }

    [Fact]
    public async Task HandleAsync_NonOwner_FailsRegardlessOfTags()
    {
        var existingPost = BlogPost.Rehydrate(
            id: 42,
            authorUserId: 99,
            categoryId: 3,
            title: "Title",
            summary: null,
            content: "Content");

        var postRepository = new FakePostRepository(existingPost);
        var handler = new EditBlogPostHandler(postRepository);

        var command = new EditBlogPostCommand(
            AuthenticatedUserId: 7,
            PostId: 42,
            Title: "Title",
            Summary: null,
            Content: "Content",
            Tags: new[] { "new-tag" });

        var result = await handler.HandleAsync(command);

        Assert.False(result.IsSuccess);
        Assert.Equal("ForbiddenPostEdit", result.ErrorCode);
    }

    private sealed class FakePostRepository(BlogPost existingPost) : IPostRepository
    {
        public Task<BlogPost> CreateAsync(BlogPost post, CancellationToken cancellationToken = default) =>
            throw new NotSupportedException();

        public Task DeleteAsync(int postId, CancellationToken cancellationToken = default) =>
            throw new NotSupportedException();

        public Task<BlogPost?> GetByIdAsync(int postId, CancellationToken cancellationToken = default) =>
            Task.FromResult<BlogPost?>(existingPost.Id == postId ? existingPost : null);

        public Task<BlogPost?> GetByIdForAuthorAsync(int postId, int authorUserId, CancellationToken cancellationToken = default) =>
            throw new NotSupportedException();

        public Task<BlogPost?> GetPublicReadByIdAsync(int postId, CancellationToken cancellationToken = default) =>
            throw new NotSupportedException();

        public Task<IReadOnlyList<BlogPost>> ListByAuthorAsync(int authorUserId, CancellationToken cancellationToken = default) =>
            throw new NotSupportedException();

        public Task<IReadOnlyList<BlogPost>> ListPublicReadAsync(CancellationToken cancellationToken = default) =>
            throw new NotSupportedException();

        public Task<BlogPost> UpdateAsync(BlogPost post, CancellationToken cancellationToken = default) =>
            Task.FromResult(post);
    }
}
