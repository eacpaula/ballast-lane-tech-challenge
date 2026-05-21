using BlogPlatform.Domain.Reactions;

namespace BlogPlatform.Domain.Tests.Reactions;

public class PostReactionTests
{
    [Theory]
    [InlineData("like", "Like")]
    [InlineData(" dislike ", "Dislike")]
    public void ReactionTypeCreate_WithSupportedValue_ReturnsReactionType(string input, string expected)
    {
        var reactionType = ReactionType.Create(input);

        Assert.Equal(expected, reactionType.Value);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData("love")]
    public void ReactionTypeCreate_WithUnsupportedValue_ThrowsArgumentException(string input)
    {
        void Action() => ReactionType.Create(input);

        var exception = Assert.Throws<ArgumentException>(Action);

        Assert.Equal("value", exception.ParamName);
    }

    [Fact]
    public void ReactionActorCreate_WithValidUserId_ReturnsUserActor()
    {
        var actor = ReactionActor.Create(userId: 7, visitorIdentifier: "visitor-1");

        Assert.Equal(7, actor.UserId);
        Assert.Null(actor.VisitorIdentifier);
    }

    [Fact]
    public void ReactionActorCreate_WithValidVisitorIdentifier_ReturnsVisitorActor()
    {
        var actor = ReactionActor.Create(userId: null, visitorIdentifier: "  visitor-1  ");

        Assert.Null(actor.UserId);
        Assert.Equal("visitor-1", actor.VisitorIdentifier);
    }

    [Fact]
    public void ReactionActorCreate_WithMissingActor_ThrowsArgumentException()
    {
        void Action() => ReactionActor.Create(userId: null, visitorIdentifier: "   ");

        var exception = Assert.Throws<ArgumentException>(Action);

        Assert.Equal("visitorIdentifier", exception.ParamName);
    }

    [Fact]
    public void PostReactionCreate_WithValidData_ReturnsReaction()
    {
        var reaction = PostReaction.Create(
            postId: 42,
            reactionType: ReactionType.Create("like"),
            actor: ReactionActor.Create(userId: null, visitorIdentifier: "visitor-1"));

        Assert.Equal(42, reaction.PostId);
        Assert.Equal("Like", reaction.ReactionType.Value);
        Assert.Null(reaction.UserId);
        Assert.Equal("visitor-1", reaction.VisitorIdentifier);
    }
}
