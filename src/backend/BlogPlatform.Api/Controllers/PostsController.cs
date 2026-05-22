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
    [HttpGet("mine")]
    [ProducesResponseType<IReadOnlyList<OwnedPostSummaryResponse>>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> ListMine(
        [FromServices] ListOwnedPostsHandler handler,
        CancellationToken cancellationToken)
    {
        var posts = await handler.HandleAsync(GetRequiredAuthenticatedUserId(), cancellationToken);
        var response = posts
            .Select(post => new OwnedPostSummaryResponse(
                post.PostId,
                post.CategoryId,
                post.Title,
                post.Summary,
                post.IsPublic,
                post.IsAvailable,
                post.Tags))
            .ToArray();

        return Ok(response);
    }

    [HttpGet("mine/{postId:int}")]
    [ProducesResponseType<OwnedPostDetailResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetMineById(
        int postId,
        [FromServices] GetOwnedPostByIdHandler handler,
        CancellationToken cancellationToken)
    {
        var result = await handler.HandleAsync(GetRequiredAuthenticatedUserId(), postId, cancellationToken);

        if (!result.IsSuccess)
        {
            return this.ProblemFromApplicationError(result.ErrorCode, result.ErrorMessage);
        }

        return Ok(new OwnedPostDetailResponse(
            result.PostId!.Value,
            result.AuthorUserId!.Value,
            result.CategoryId!.Value,
            result.Title!,
            result.Summary,
            result.Content!,
            result.IsPublic!.Value,
            result.IsAvailable!.Value,
            result.Tags));
    }

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
                request.Content,
                request.Tags),
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
            result.Content,
            result.Tags));
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
                request.Content,
                request.Tags),
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
            result.Content,
            result.Tags));
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
            Content: null,
            Tags: Array.Empty<string>()));
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
