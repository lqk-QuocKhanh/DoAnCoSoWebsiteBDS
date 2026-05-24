using System.Net;
using System.Text.Json;
using VietPropEstate.Application.Common.Exceptions;
using VietPropEstate.Domain.Exceptions;
using ValidationException = VietPropEstate.Application.Common.Exceptions.ValidationException;

namespace VietPropEstate.WebAPI.Middleware;

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
            await HandleExceptionAsync(context, ex);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        if (exception is UnauthorizedAccessException unauthorized)
        {
            _logger.LogWarning("Authentication failed: {Message}", unauthorized.Message);
        }
        else
        {
            _logger.LogError(exception, "An unhandled exception occurred: {Message}", exception.Message);
        }

        var (statusCode, title, errors) = exception switch
        {
            ValidationException ve => (
                HttpStatusCode.BadRequest,
                "Validation Failed",
                (object)ve.Errors),

            NotFoundException nfe => (
                HttpStatusCode.NotFound,
                "Resource Not Found",
                (object)new { message = nfe.Message }),

            DomainException de => (
                HttpStatusCode.UnprocessableEntity,
                "Business Rule Violation",
                (object)new { message = de.Message }),

            ForbiddenAccessException fae => (
                HttpStatusCode.Forbidden,
                "Forbidden",
                (object)new
                {
                    message = string.IsNullOrWhiteSpace(fae.Message)
                        ? "You do not have permission to perform this action."
                        : fae.Message,
                    code = fae.ErrorCode
                }),

            UnauthorizedAccessException uae => (
                HttpStatusCode.Unauthorized,
                "Unauthorized",
                (object)new { message = uae.Message }),

            InvalidOperationException ioe => (
                HttpStatusCode.BadRequest,
                "Bad Request",
                (object)new { message = ioe.Message }),

            _ => (
                HttpStatusCode.InternalServerError,
                "Internal Server Error",
                (object)new { message = "An unexpected error occurred. Please try again later." })
        };

        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)statusCode;

        var response = new
        {
            type = $"https://tools.ietf.org/html/rfc9110#section-{(int)statusCode}",
            title,
            status = (int)statusCode,
            errors,
            traceId = context.TraceIdentifier
        };

        var json = JsonSerializer.Serialize(response, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        });

        await context.Response.WriteAsync(json);
    }
}
