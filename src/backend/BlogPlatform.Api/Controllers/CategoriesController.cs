using System.Security.Claims;
using BlogPlatform.Api.Contracts.Categories;
using BlogPlatform.Api.Errors;
using BlogPlatform.Application.Posts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BlogPlatform.Api.Controllers;

[ApiController]
[Authorize(Policy = "AdministratorOnly")]
[Route("api/categories")]
public sealed class CategoriesController : ControllerBase
{
    [HttpGet]
    [ProducesResponseType<IReadOnlyList<AdminCategoryListItemResponse>>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> List(
        [FromServices] ListAllPostCategoriesHandler handler,
        CancellationToken cancellationToken)
    {
        var categories = await handler.HandleAsync(IsAdministrator(), cancellationToken);
        var response = categories
            .Select(category => new AdminCategoryListItemResponse(
                category.CategoryId,
                category.Title,
                category.IsAvailable))
            .ToArray();

        return Ok(response);
    }

    [HttpPost]
    [ProducesResponseType<CategoryResponse>(StatusCodes.Status201Created)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Create(
        [FromBody] CreateCategoryRequest request,
        [FromServices] CreatePostCategoryHandler handler,
        CancellationToken cancellationToken)
    {
        var result = await handler.HandleAsync(
            new CreatePostCategoryCommand(GetRequiredAuthenticatedUserId(), IsAdministrator(), request.Title),
            cancellationToken);

        if (!result.IsSuccess)
        {
            return this.ProblemFromApplicationError(result.ErrorCode, result.ErrorMessage);
        }

        return Created($"/api/categories/{result.CategoryId}", new CategoryResponse(
            result.CategoryId!.Value,
            result.Title!,
            result.IsAvailable!.Value));
    }

    [HttpPut("{categoryId:int}")]
    [ProducesResponseType<CategoryResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Update(
        int categoryId,
        [FromBody] UpdateCategoryRequest request,
        [FromServices] UpdatePostCategoryHandler handler,
        CancellationToken cancellationToken)
    {
        var result = await handler.HandleAsync(
            new UpdatePostCategoryCommand(GetRequiredAuthenticatedUserId(), IsAdministrator(), categoryId, request.Title),
            cancellationToken);

        if (!result.IsSuccess)
        {
            return this.ProblemFromApplicationError(result.ErrorCode, result.ErrorMessage);
        }

        return Ok(new CategoryResponse(
            result.CategoryId!.Value,
            result.Title!,
            result.IsAvailable!.Value));
    }

    [HttpDelete("{categoryId:int}")]
    [ProducesResponseType<CategoryResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Deactivate(
        int categoryId,
        [FromServices] DeactivatePostCategoryHandler handler,
        CancellationToken cancellationToken)
    {
        var result = await handler.HandleAsync(
            new DeactivatePostCategoryCommand(GetRequiredAuthenticatedUserId(), IsAdministrator(), categoryId),
            cancellationToken);

        if (!result.IsSuccess)
        {
            return this.ProblemFromApplicationError(result.ErrorCode, result.ErrorMessage);
        }

        return Ok(new CategoryResponse(
            result.CategoryId!.Value,
            result.Title!,
            result.IsAvailable!.Value));
    }

    private bool IsAdministrator() => User.IsInRole("Administrator");

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
