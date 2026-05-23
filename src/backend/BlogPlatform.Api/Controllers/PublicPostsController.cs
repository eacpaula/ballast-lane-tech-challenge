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
    [ProducesResponseType<PaginatedPublicPostResponse>(StatusCodes.Status200OK)]
    public async Task<IActionResult> List(
        [FromQuery(Name = "q")] string? query,
        [FromQuery] int? page,
        [FromQuery] int? pageSize,
        [FromServices] ListPublicPostsHandler handler,
        CancellationToken cancellationToken)
    {
        try
        {
            var posts = await handler.HandleAsync(
                query,
                page ?? PostListPageRequest.DefaultPage,
                pageSize ?? PostListPageRequest.DefaultPageSize,
                TryGetAuthenticatedUserId(),
                cancellationToken);
            var response = new PaginatedPublicPostResponse(
                posts.Items
                    .Select(post => new PublicPostSummaryResponse(
                        post.PostId,
                        post.Title,
                        post.Summary,
                        post.Tags,
                        post.PublishDate,
                        post.ExpirationDate))
                    .ToArray(),
                posts.Page,
                posts.PageSize,
                posts.TotalCount,
                posts.TotalPages,
                posts.HasNextPage);

            return Ok(response);
        }
        catch (ArgumentOutOfRangeException exception)
        {
            return ValidationProblem(new ValidationProblemDetails(new Dictionary<string, string[]>
            {
                [exception.ParamName ?? "page"] = [exception.Message],
            }));
        }
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
            result.Content!,
            result.Tags,
            result.PublishDate,
            result.ExpirationDate));
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
