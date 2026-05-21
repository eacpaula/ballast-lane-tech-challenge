using BlogPlatform.Api.Contracts.Categories;
using BlogPlatform.Application.Posts;
using Microsoft.AspNetCore.Mvc;

namespace BlogPlatform.Api.Controllers;

[ApiController]
[Route("api/categories/available")]
public sealed class PublicCategoriesController : ControllerBase
{
    [HttpGet]
    [ProducesResponseType<IReadOnlyList<AvailableCategoryResponse>>(StatusCodes.Status200OK)]
    public async Task<IActionResult> ListAvailable(
        [FromServices] ListAvailablePostCategoriesHandler handler,
        CancellationToken cancellationToken)
    {
        var categories = await handler.HandleAsync(cancellationToken);
        var response = categories
            .Select(category => new AvailableCategoryResponse(category.CategoryId, category.Title))
            .ToArray();

        return Ok(response);
    }
}
