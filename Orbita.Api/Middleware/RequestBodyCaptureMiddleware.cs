using System.Text;

namespace Orbita.Api.Middleware;

public class RequestBodyCaptureMiddleware
{
    private const int MaxBodyLength = 64 * 1024; // 64 KB
    public const string RequestBodyKey = "CapturedRequestBody";

    private readonly RequestDelegate _next;

    public RequestBodyCaptureMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext httpContext)
    {
        var request = httpContext.Request;

        if (request.ContentLength is > 0 and <= MaxBodyLength)
        {
            request.EnableBuffering();
            request.Body.Position = 0;
            using var reader = new StreamReader(request.Body, Encoding.UTF8, leaveOpen: true);
            var body = await reader.ReadToEndAsync();
            request.Body.Position = 0;
            httpContext.Items[RequestBodyKey] = body;
        }

        await _next(httpContext);
    }
}
