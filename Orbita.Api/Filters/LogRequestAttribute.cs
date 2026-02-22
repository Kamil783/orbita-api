using System.Diagnostics;
using System.Security.Claims;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Orbita.Api.Middleware;
using Orbita.Application.Abstractions;
using Orbita.Application.Models;
using Orbita.Application.Models.Log;

namespace Orbita.Api.Filters;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
public class LogRequestAttribute : ActionFilterAttribute
{
    private const int MaxBodyLength = 64 * 1024; // 64 KB

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        WriteIndented = false,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    /// <summary>
    /// Whether to log the request body. Disable for file upload endpoints.
    /// </summary>
    public bool LogBody { get; set; } = true;

    /// <summary>
    /// Whether to log the response body.
    /// </summary>
    public bool LogResponse { get; set; } = true;

    public override async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        var httpContext = context.HttpContext;
        var request = httpContext.Request;
        var stopwatch = Stopwatch.StartNew();

        // Read request body captured by RequestBodyCaptureMiddleware (runs before model binding)
        string? requestBody = null;
        if (LogBody && httpContext.Items.TryGetValue(RequestBodyCaptureMiddleware.RequestBodyKey, out var cached))
        {
            requestBody = cached as string;
        }

        // Collect selected request headers
        var headers = new Dictionary<string, string>();
        foreach (var key in new[] { "Content-Type", "Accept", "Accept-Language", "Origin", "Referer" })
        {
            if (request.Headers.TryGetValue(key, out var value))
                headers[key] = value.ToString();
        }

        if (request.Headers.ContainsKey("Authorization"))
            headers["Authorization"] = "***";

        var headersJson = headers.Count > 0 ? JsonSerializer.Serialize(headers, JsonOptions) : null;

        // Execute the action
        var executedContext = await next();

        stopwatch.Stop();

        // Capture response info
        int statusCode;
        string? responseBody = null;

        if (executedContext.Exception != null && !executedContext.ExceptionHandled)
        {
            statusCode = 500;
        }
        else if (executedContext.Result is ObjectResult objectResult)
        {
            statusCode = objectResult.StatusCode ?? 200;
            if (LogResponse && objectResult.Value != null)
            {
                try
                {
                    responseBody = JsonSerializer.Serialize(objectResult.Value, JsonOptions);
                    if (responseBody.Length > MaxBodyLength)
                        responseBody = responseBody[..MaxBodyLength] + "...[truncated]";
                }
                catch
                {
                    responseBody = "[serialization failed]";
                }
            }
        }
        else if (executedContext.Result is StatusCodeResult statusCodeResult)
        {
            statusCode = statusCodeResult.StatusCode;
        }
        else
        {
            statusCode = httpContext.Response.StatusCode;
        }

        // Get user info from claims
        var userId = httpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);

        // Get client IP
        var clientIp = httpContext.Connection.RemoteIpAddress?.ToString();
        if (request.Headers.TryGetValue("X-Forwarded-For", out var forwardedFor))
            clientIp = forwardedFor.ToString().Split(',')[0].Trim();

        var entry = new RequestLogEntry
        {
            TraceId = Activity.Current?.TraceId.ToString() ?? httpContext.TraceIdentifier,
            HttpMethod = request.Method,
            Path = request.Path.Value ?? "/",
            QueryString = request.QueryString.HasValue ? request.QueryString.Value : null,
            RequestBody = requestBody,
            RequestHeaders = headersJson,
            ResponseStatusCode = statusCode,
            ResponseBody = responseBody,
            UserId = userId,
            ClientIp = clientIp,
            UserAgent = request.Headers.UserAgent.ToString(),
            ControllerName = context.Controller.GetType().Name,
            ActionName = context.ActionDescriptor.DisplayName,
            DurationMs = stopwatch.ElapsedMilliseconds
        };

        var logger = httpContext.RequestServices.GetRequiredService<IAppLogger>();
        await logger.LogRequestAsync(entry);
    }
}
