using System.Net;
using System.Text.Json;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using specmatic_uuid_api.Models;

public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An unhandled exception occurred");
            await HandleExceptionAsync(context, ex);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/json";

        var response = exception switch
        {
            BadHttpRequestException badRequestEx => new ErrorResponse
            {
                TimeStamp = DateTime.UtcNow.ToString("o"),
                Error = "Bad Request",
                Message = badRequestEx.Message,
            },
            KeyNotFoundException notFoundEx => new ErrorResponse
            {
                TimeStamp = DateTime.UtcNow.ToString("o"),
                Error = "Not Found",
                Message = notFoundEx.Message
            },
            _ => new ErrorResponse
            {
                TimeStamp = DateTime.UtcNow.ToString("o"),
                Error = "Internal Server Error",
                Message = "An unexpected error occurred",
            }
        };

        var statusCode = exception switch
        {
            BadHttpRequestException => HttpStatusCode.BadRequest,
            KeyNotFoundException => HttpStatusCode.NotFound,
            _ => HttpStatusCode.InternalServerError
        };

        context.Response.StatusCode = (int)statusCode;
        await context.Response.WriteAsync(JsonSerializer.Serialize(response));
    }
}
