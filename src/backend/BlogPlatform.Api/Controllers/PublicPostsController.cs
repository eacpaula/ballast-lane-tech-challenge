using BlogPlatform.Api.Contracts.Posts;
using BlogPlatform.Api.Errors;
using BlogPlatform.Application.Posts;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;

namespace BlogPlatform.Api.Controllers;

[ApiController]
[Route("api/posts")]
public sealed class PublicPostsController : ControllerBase
{
    [HttpGet]
    [ProducesResponseType<IReadOnlyList<PublicPostSummaryResponse>>(StatusCodes.Status200OK)]
    public async Task<IActionResult> List(
        [FromQuery(Name = "q")] string? query,
        [FromServices] ListPublicPostsHandler handler,
        CancellationToken cancellationToken)
    {
        var posts = await handler.HandleAsync(query, TryGetAuthenticatedUserId(), cancellationToken);
        var response = posts
            .Select(post => new PublicPostSummaryResponse(post.PostId, post.Title, post.Summary))
            .ToArray();

        return Ok(response);
    }

    [HttpGet("{postId:int}")]
    [ProducesResponseType<PublicPostDetailResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(
        int postId,
        [FromServices] GetPublicPostByIdHandler handler,
        CancellationToken cancellationToken)
    {
        var result = await handler.HandleAsync(postId, cancellationToken);

        if (!result.IsSuccess)
        {
            return this.ProblemFromApplicationError(result.ErrorCode, result.ErrorMessage);
        }

        return Ok(new PublicPostDetailResponse(
            result.PostId!.Value,
            result.Title!,
            result.Summary,
            result.Content!));
    }

    private int? TryGetAuthenticatedUserId()
    {
        if (!(User.Identity?.IsAuthenticated ?? false))
        {
            return null;
        }

        var rawValue = User.FindFirstValue(ClaimTypes.NameIdentifier);
        return int.TryParse(rawValue, out var userId) && userId > 0
            ? userId
            : null;
    }
}
