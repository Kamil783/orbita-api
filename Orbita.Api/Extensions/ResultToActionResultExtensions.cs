using Microsoft.AspNetCore.Mvc;
using Orbita.Api.Factories;
using Orbita.Application.Models.Results;

namespace Orbita.Api.Extensions;

public static class ResultToActionResultExtensions
{
    public static IActionResult ToActionResult(this Result result, HttpContext httpContext)
    {
        if (result.IsSuccess)
            return new OkResult();

        var (status, body) = ErrorResponseFactory.FromResult(result, httpContext);
        return new ObjectResult(body) { StatusCode = status };
    }

    public static IActionResult ToActionResult<T>(this Result<T> result, HttpContext httpContext)
    {
        if (result.IsSuccess)
            return new OkObjectResult(result.Value);

        return ((Result)result).ToActionResult(httpContext);
    }
}
