using System.Security.Claims;
using BlogPlatform.Api.Contracts.Posts;
using BlogPlatform.Api.Errors;
using BlogPlatform.Application.Posts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BlogPlatform.Api.Controllers;

[ApiController]
[Authorize]
[Route("api/posts")]
public sealed class PostsController : ControllerBase
{
    [HttpPost]
    [ProducesResponseType<PostMutationResponse>(StatusCodes.Status201Created)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Create(
        [FromBody] CreatePostRequest request,
        [FromServices] CreateBlogPostHandler handler,
        CancellationToken cancellationToken)
    {
        var result = await handler.HandleAsync(
            new CreateBlogPostCommand(
                GetRequiredAuthenticatedUserId(),
                request.CategoryId,
                request.Title,
                request.Summary,
                request.Content),
            cancellationToken);

        if (!result.IsSuccess)
        {
            return this.ProblemFromApplicationError(result.ErrorCode, result.ErrorMessage);
        }

        return Created($"/api/posts/{result.PostId}", new PostMutationResponse(
            result.PostId!.Value,
            result.AuthorUserId!.Value,
            result.Title!,
            result.Summary,
            result.Content));
    }

    [HttpPut("{postId:int}")]
    [ProducesResponseType<PostMutationResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status403Forbidden)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(
        int postId,
        [FromBody] UpdatePostRequest request,
        [FromServices] EditBlogPostHandler handler,
        CancellationToken cancellationToken)
    {
        var result = await handler.HandleAsync(
            new EditBlogPostCommand(
                GetRequiredAuthenticatedUserId(),
                postId,
                request.Title,
                request.Summary,
                request.Content),
            cancellationToken);

        if (!result.IsSuccess)
        {
            return this.ProblemFromApplicationError(result.ErrorCode, result.ErrorMessage);
        }

        return Ok(new PostMutationResponse(
            result.PostId!.Value,
            result.AuthorUserId!.Value,
            result.Title!,
            result.Summary,
            result.Content));
    }

    [HttpDelete("{postId:int}")]
    [ProducesResponseType<PostMutationResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status403Forbidden)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(
        int postId,
        [FromServices] RemoveBlogPostHandler handler,
        CancellationToken cancellationToken)
    {
        var result = await handler.HandleAsync(
            new RemoveBlogPostCommand(GetRequiredAuthenticatedUserId(), postId),
            cancellationToken);

        if (!result.IsSuccess)
        {
            return this.ProblemFromApplicationError(result.ErrorCode, result.ErrorMessage);
        }

        return Ok(new PostMutationResponse(
            result.PostId!.Value,
            result.AuthorUserId!.Value,
            Title: string.Empty,
            Summary: null,
            Content: null));
    }

    private int GetRequiredAuthenticatedUserId()
    {
        var rawValue = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (!int.TryParse(rawValue, out var userId) || userId <= 0)
        {
            throw new InvalidOperationException("Authenticated user id claim is missing.");
        }

        return userId;
    }
}
