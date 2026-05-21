using Microsoft.AspNetCore.Mvc;

namespace BlogPlatform.Api.Errors;

public static class ProblemDetailsFactoryExtensions
{
    public static ProblemDetails CreateProblemDetails(
        this HttpContext httpContext,
        int statusCode,
        string title,
        string detail)
    {
        ArgumentNullException.ThrowIfNull(httpContext);

        return new ProblemDetails
        {
            Status = statusCode,
            Title = title,
            Detail = detail,
            Instance = httpContext.Request.Path,
            Type = $"https://httpstatuses.com/{statusCode}",
        };
    }

    public static async Task WriteProblemDetailsAsync(this HttpContext httpContext, ProblemDetails problemDetails)
    {
        ArgumentNullException.ThrowIfNull(httpContext);
        ArgumentNullException.ThrowIfNull(problemDetails);

        httpContext.Response.StatusCode = problemDetails.Status ?? StatusCodes.Status500InternalServerError;
        httpContext.Response.ContentType = "application/problem+json";
        await httpContext.Response.WriteAsJsonAsync(problemDetails);
    }

    public static ObjectResult ProblemFromApplicationError(
        this ControllerBase controller,
        string? errorCode,
        string? errorMessage)
    {
        ArgumentNullException.ThrowIfNull(controller);

        var (statusCode, title) = errorCode switch
        {
            "ValidationError" => (StatusCodes.Status400BadRequest, "Validation Error"),
            "InvalidReactionType" => (StatusCodes.Status400BadRequest, "Validation Error"),
            "InvalidReactionActor" => (StatusCodes.Status400BadRequest, "Validation Error"),
            "DuplicateEmail" => (StatusCodes.Status409Conflict, "Conflict"),
            "DuplicateCategoryTitle" => (StatusCodes.Status409Conflict, "Conflict"),
            "InvalidCredentials" => (StatusCodes.Status401Unauthorized, "Unauthorized"),
            "AuthenticationRequired" => (StatusCodes.Status401Unauthorized, "Unauthorized"),
            "AuthorizationRequired" => (StatusCodes.Status403Forbidden, "Forbidden"),
            "ForbiddenPostEdit" => (StatusCodes.Status403Forbidden, "Forbidden"),
            "ForbiddenPostRemoval" => (StatusCodes.Status403Forbidden, "Forbidden"),
            "PostNotFound" => (StatusCodes.Status404NotFound, "Not Found"),
            "PostNotAvailable" => (StatusCodes.Status404NotFound, "Not Found"),
            "CategoryNotFound" => (StatusCodes.Status404NotFound, "Not Found"),
            "CategoryNotFoundOrUnavailable" => (StatusCodes.Status404NotFound, "Not Found"),
            _ => (StatusCodes.Status500InternalServerError, "Unexpected Error"),
        };

        var problem = controller.HttpContext.CreateProblemDetails(
            statusCode: statusCode,
            title: title,
            detail: errorMessage ?? "The request could not be processed.");

        return new ObjectResult(problem)
        {
            StatusCode = statusCode,
        };
    }
}
