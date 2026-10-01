using System.Net;
using System.Text.Json;
using GoFan.Application.DTOs.Common;
using GoFan.Application.Exceptions;

namespace GoFan.Middlewares;

public class GlobalExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<GlobalExceptionMiddleware> _logger;
    private readonly IHostEnvironment _env;

    public GlobalExceptionMiddleware(RequestDelegate next, ILogger<GlobalExceptionMiddleware> logger, IHostEnvironment env)
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
            await HandleExceptionAsync(context, ex);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/json";

        int statusCode;
        string message;
        List<string>? errors = null;

        if (exception is AppException appEx)
        {
            statusCode = (int)appEx.StatusCode;
            message = appEx.Message;
            errors = appEx.Errors;
            _logger.LogWarning(appEx, "Handled application exception: {Message}", appEx.Message);
        }
        else
        {
            statusCode = (int)HttpStatusCode.InternalServerError;
            message = _env.IsDevelopment()
                ? exception.Message
                : "An unexpected error occurred. Please try again later.";
            
            if (_env.IsDevelopment() && exception.StackTrace != null)
            {
                errors = new List<string> { exception.StackTrace };
            }

            _logger.LogError(exception, "Unhandled exception occurred: {Message}", exception.Message);
        }

        context.Response.StatusCode = statusCode;

        var response = ApiResponse<object>.Fail(message, statusCode, errors);

        var jsonOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };

        await context.Response.WriteAsync(JsonSerializer.Serialize(response, jsonOptions));
    }
}
