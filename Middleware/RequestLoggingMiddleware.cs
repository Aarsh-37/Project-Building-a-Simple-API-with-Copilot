namespace UserManagementAPI.Middleware;

public class RequestLoggingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<RequestLoggingMiddleware> _logger;

    public RequestLoggingMiddleware(
        RequestDelegate next,
        ILogger<RequestLoggingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var startedAt = DateTimeOffset.UtcNow;
        _logger.LogInformation(
            "HTTP {Method} {Path} started at {StartedAt}",
            context.Request.Method,
            context.Request.Path,
            startedAt);

        await _next(context);

        _logger.LogInformation(
            "HTTP {Method} {Path} responded with {StatusCode} in {ElapsedMs} ms",
            context.Request.Method,
            context.Request.Path,
            context.Response.StatusCode,
            (DateTimeOffset.UtcNow - startedAt).TotalMilliseconds);
    }
}
