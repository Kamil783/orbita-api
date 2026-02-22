using Orbita.Api.Factories;
using Orbita.Application.Abstractions;
using System.Text.Json;

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

    private async Task HandleExceptionAsync(HttpContext ctx, Exception ex)
    {
        _logger.LogError(ex, "Unhandled exception on {Method} {Path}", ctx.Request.Method, ctx.Request.Path);

        try
        {
            var appLogger = ctx.RequestServices.GetService<IAppLogger>();
            appLogger?.LogError(
                $"Unhandled exception on {ctx.Request.Method} {ctx.Request.Path}",
                ex,
                source: "ExceptionHandlingMiddleware");
        }
        catch
        {

        }

        var env = ctx.RequestServices.GetRequiredService<IHostEnvironment>();
        var isDev = env.IsDevelopment();

        var (status, body) = ErrorResponseFactory.FromException(ex, ctx, isDev);

        ctx.Response.ContentType = "application/json";
        ctx.Response.StatusCode = status;

        await ctx.Response.WriteAsync(JsonSerializer.Serialize(body, JsonOptions));
    }
}