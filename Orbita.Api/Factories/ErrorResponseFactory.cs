using Orbita.Application.Models.Results;

namespace Orbita.Api.Factories;

public static class ErrorResponseFactory
{
    public static (int statusCode, object body) FromResult(Result result, HttpContext ctx, bool includeDetails = true)
    {
        var error = result.Error!;

        var statusCode = error.Type switch
        {
            ErrorType.Validation => StatusCodes.Status400BadRequest,
            ErrorType.NotFound => StatusCodes.Status404NotFound,
            ErrorType.Conflict => StatusCodes.Status409Conflict,
            ErrorType.Unauthorized => StatusCodes.Status401Unauthorized,
            ErrorType.Forbidden => StatusCodes.Status403Forbidden,
            _ => StatusCodes.Status500InternalServerError
        };

        var body = new
        {
            title = TitleFromErrorType(error.Type),
            status = statusCode,
            code = error.Code,
            detail = includeDetails ? error.Message : "An error occurred.",
            traceId = ctx.TraceIdentifier,
            path = ctx.Request.Path.Value,
            method = ctx.Request.Method,
            errors = error.Type == ErrorType.Validation ? error.ValidationErrors : null
        };

        return (statusCode, body);
    }

    public static (int statusCode, object body) FromException(Exception ex, HttpContext ctx, bool isDevelopment)
    {
        var statusCode = ex switch
        {
            ArgumentException => StatusCodes.Status400BadRequest,
            UnauthorizedAccessException => StatusCodes.Status401Unauthorized,
            KeyNotFoundException => StatusCodes.Status404NotFound,
            InvalidOperationException => StatusCodes.Status409Conflict,
            _ => StatusCodes.Status500InternalServerError
        };

        var title = statusCode switch
        {
            StatusCodes.Status400BadRequest => "Bad Request",
            StatusCodes.Status401Unauthorized => "Unauthorized",
            StatusCodes.Status404NotFound => "Not Found",
            StatusCodes.Status409Conflict => "Conflict",
            _ => "Internal Server Error"
        };

        var body = new
        {
            title,
            status = statusCode,
            code = "unexpected_error",
            detail = isDevelopment ? ex.Message : "An unexpected error occurred.",
            traceId = ctx.TraceIdentifier,
            path = ctx.Request.Path.Value,
            method = ctx.Request.Method,
            stackTrace = isDevelopment ? ex.StackTrace : null
        };

        return (statusCode, body);
    }

    private static string TitleFromErrorType(ErrorType type) => type switch
    {
        ErrorType.Validation => "Validation error",
        ErrorType.NotFound => "Not found",
        ErrorType.Conflict => "Conflict",
        ErrorType.Unauthorized => "Unauthorized",
        ErrorType.Forbidden => "Forbidden",
        _ => "Server error"
    };
}
