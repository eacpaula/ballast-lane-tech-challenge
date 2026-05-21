using BlogPlatform.Domain.Reactions;

namespace BlogPlatform.Application.Posts;

public sealed class ReactToPostResult
{
    private ReactToPostResult(
        bool isSuccess,
        int? postId,
        string? reactionType,
        int? userId,
        string? visitorIdentifier,
        string? errorCode,
        string? errorMessage)
    {
        IsSuccess = isSuccess;
        PostId = postId;
        ReactionType = reactionType;
        UserId = userId;
        VisitorIdentifier = visitorIdentifier;
        ErrorCode = errorCode;
        ErrorMessage = errorMessage;
    }

    public bool IsSuccess { get; }

    public int? PostId { get; }

    public string? ReactionType { get; }

    public int? UserId { get; }

    public string? VisitorIdentifier { get; }

    public string? ErrorCode { get; }

    public string? ErrorMessage { get; }

    public static ReactToPostResult Success(PostReaction reaction)
    {
        return new ReactToPostResult(
            isSuccess: true,
            postId: reaction.PostId,
            reactionType: reaction.ReactionType.Value,
            userId: reaction.UserId,
            visitorIdentifier: reaction.VisitorIdentifier,
            errorCode: null,
            errorMessage: null);
    }

    public static ReactToPostResult Failure(string errorCode, string errorMessage)
    {
        return new ReactToPostResult(
            isSuccess: false,
            postId: null,
            reactionType: null,
            userId: null,
            visitorIdentifier: null,
            errorCode: errorCode,
            errorMessage: errorMessage);
    }
}
