using System.Net;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;

namespace HRManagement.Api.Middleware;

public class GlobalExceptionHandlerMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<GlobalExceptionHandlerMiddleware> _logger;
    private readonly IHostEnvironment _env;

    public GlobalExceptionHandlerMiddleware(
        RequestDelegate next,
        ILogger<GlobalExceptionHandlerMiddleware> logger,
        IHostEnvironment env)
    {
        _next = next;
        _logger = logger;
        _env = env;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            if (context.Response.HasStarted)
            {
                _logger.LogWarning(ex, "Response already started, rethrowing exception.");
                throw;
            }
            await HandleExceptionAsync(context, ex);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception ex)
    {
        var (statusCode, title) = MapExceptionToStatus(ex);
        var traceId = context.TraceIdentifier;
        var exceptionType = ex.GetType().Name;
        var detail = BuildDetail(ex, statusCode);

        _logger.LogError(ex, "Unhandled exception. TraceId: {TraceId}, Status: {Status}, Type: {Type}", traceId, statusCode, exceptionType);

        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)statusCode;

        var problem = new ProblemDetails
        {
            Type = "https://tools.ietf.org/html/rfc7231#section-6.5.1",
            Title = title,
            Status = (int)statusCode,
            Detail = detail,
            Instance = context.Request.Path,
            Extensions =
            {
                ["traceId"] = traceId,
                ["exceptionType"] = exceptionType
            }
        };

        if (!_env.IsDevelopment() && statusCode == HttpStatusCode.InternalServerError)
        {
            problem.Extensions["hint"] = "Provide the traceId when contacting support.";
        }

        var json = JsonSerializer.Serialize(problem, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });
        await context.Response.WriteAsync(json);
    }

    private string BuildDetail(Exception ex, HttpStatusCode statusCode)
    {
        var message = ex.Message;
        if (ex.InnerException != null)
            message += " Inner: " + ex.InnerException.Message;

        if (_env.IsDevelopment())
            return message;

        if (statusCode == HttpStatusCode.InternalServerError)
            return "An unexpected error occurred. Please provide the traceId when contacting support.";

        return message;
    }

    private static (HttpStatusCode statusCode, string title) MapExceptionToStatus(Exception ex)
    {
        return ex switch
        {
            KeyNotFoundException => (HttpStatusCode.NotFound, "The requested resource was not found."),
            ArgumentNullException => (HttpStatusCode.BadRequest, "A required value was null or missing."),
            ArgumentException => (HttpStatusCode.BadRequest, "An invalid or missing argument was supplied."),
            InvalidOperationException => (HttpStatusCode.BadRequest, "The operation is not valid in the current state."),
            UnauthorizedAccessException => (HttpStatusCode.Unauthorized, "Access denied. Authentication or authorization failed."),
            _ => (HttpStatusCode.InternalServerError, "An unexpected error occurred while processing your request.")
        };
    }
}
