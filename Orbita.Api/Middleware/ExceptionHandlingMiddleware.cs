using System.Net;
using System.Text.Json;
using Orbita.Application.Abstractions;

namespace Orbita.Api.Middleware;

public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        WriteIndented = false
    };

    public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext httpContext)
    {
        try
        {
            await _next(httpContext);
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(httpContext, ex);
        }
    }

    private async Task HandleExceptionAsync(HttpContext httpContext, Exception exception)
    {
        _logger.LogError(exception,
            "Unhandled exception on {Method} {Path}",
            httpContext.Request.Method,
            httpContext.Request.Path);

        // Log to database via IAppLogger
        try
        {
            var appLogger = httpContext.RequestServices.GetService<IAppLogger>();
            if (appLogger != null)
            {
                appLogger.LogError(
                    $"Unhandled exception on {httpContext.Request.Method} {httpContext.Request.Path}",
                    exception,
                    source: "ExceptionHandlingMiddleware");
            }
        }
        catch
        {
            // Don't let logging failures mask the original exception
        }

        var (statusCode, title) = exception switch
        {
            ArgumentException => (HttpStatusCode.BadRequest, "Bad Request"),
            UnauthorizedAccessException => (HttpStatusCode.Unauthorized, "Unauthorized"),
            KeyNotFoundException => (HttpStatusCode.NotFound, "Not Found"),
            InvalidOperationException => (HttpStatusCode.Conflict, "Conflict"),
            _ => (HttpStatusCode.InternalServerError, "Internal Server Error")
        };

        var env = httpContext.RequestServices.GetRequiredService<IHostEnvironment>();
        var isDevelopment = env.IsDevelopment();

        var problemDetails = new
        {
            type = $"https://httpstatuses.io/{(int)statusCode}",
            title,
            status = (int)statusCode,
            detail = isDevelopment ? exception.Message : "An unexpected error occurred.",
            traceId = httpContext.TraceIdentifier,
            stackTrace = isDevelopment ? exception.StackTrace : null
        };

        httpContext.Response.ContentType = "application/problem+json";
        httpContext.Response.StatusCode = (int)statusCode;

        await httpContext.Response.WriteAsync(JsonSerializer.Serialize(problemDetails, JsonOptions));
    }
}
