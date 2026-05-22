using BlogPlatform.Api.Contracts.Categories;
using BlogPlatform.Application.Posts;
using Microsoft.AspNetCore.Mvc;

namespace BlogPlatform.Api.Controllers;

[ApiController]
[Route("api/categories/available")]
public sealed class PublicCategoriesController : ControllerBase
{
    [HttpGet]
    [ProducesResponseType<PaginatedCategoryResponse<AvailableCategoryResponse>>(StatusCodes.Status200OK)]
    public async Task<IActionResult> ListAvailable(
        [FromQuery] int? page,
        [FromQuery] int? pageSize,
        [FromServices] ListAvailablePostCategoriesHandler handler,
        CancellationToken cancellationToken)
    {
        try
        {
            var categories = await handler.HandleAsync(page, pageSize, cancellationToken);
            var response = new PaginatedCategoryResponse<AvailableCategoryResponse>(
                categories.Items
                    .Select(category => new AvailableCategoryResponse(
                        category.CategoryId,
                        category.Title,
                        category.Description,
                        category.IsAvailable))
                    .ToArray(),
                categories.Page,
                categories.PageSize,
                categories.TotalCount,
                categories.TotalPages,
                categories.HasNextPage);

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
}
