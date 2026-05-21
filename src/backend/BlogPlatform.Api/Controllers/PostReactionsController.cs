using System.Security.Claims;
using BlogPlatform.Api.Contracts.Reactions;
using BlogPlatform.Api.Errors;
using BlogPlatform.Application.Posts;
using Microsoft.AspNetCore.Mvc;

namespace BlogPlatform.Api.Controllers;

[ApiController]
[Route("api/posts/{postId:int}/reactions")]
public sealed class PostReactionsController : ControllerBase
{
    [HttpPost]
    [ProducesResponseType<ReactionResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> React(
        int postId,
        [FromBody] ReactToPostRequest request,
        [FromServices] ReactToPostHandler handler,
        CancellationToken cancellationToken)
    {
        var result = await handler.HandleAsync(
            new ReactToPostCommand(
                postId,
                request.ReactionType,
                GetOptionalAuthenticatedUserId(),
                request.VisitorIdentifier),
            cancellationToken);

        if (!result.IsSuccess)
        {
            return this.ProblemFromApplicationError(result.ErrorCode, result.ErrorMessage);
        }

        return Ok(new ReactionResponse(
            result.PostId!.Value,
            result.ReactionType!,
            result.UserId,
            result.VisitorIdentifier));
    }

    private int? GetOptionalAuthenticatedUserId()
    {
        var rawValue = User.FindFirstValue(ClaimTypes.NameIdentifier);
        return int.TryParse(rawValue, out var userId) ? userId : null;
    }
}
