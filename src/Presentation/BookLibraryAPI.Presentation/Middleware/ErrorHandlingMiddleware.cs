using BookLibraryAPI.Core.Domain.Common.Exceptions;

namespace BookLibraryAPI.Presentation.Middleware;

using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Text.Json;

public class ErrorHandlingMiddleware(RequestDelegate next, ILogger<ErrorHandlingMiddleware> logger)
{
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Unhandled exception: {Message} | Path: {Path}",
                ex.Message, context.Request.Path);

            await HandleExceptionAsync(context, ex);
        }
    }

    private static async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/problem+json";

        var problemDetails = exception switch
        {
            DomainException domainEx => new ProblemDetails
            {
                Type = $"https://httpstatuses.com/400",
                Title = "Domain Rule Violation",
                Status = StatusCodes.Status400BadRequest,
                Detail = domainEx.Message,
                Extensions =
                {
                    ["errorCode"] = domainEx.ErrorCode,
                    ["property"] = domainEx.PropertyName,
                    ["traceId"] = context.TraceIdentifier,
                    ["additionalData"] = domainEx.AdditionalData
                }
            },
            
            ValidationException validationEx => new ProblemDetails
            {
                Type = $"https://httpstatuses.com/400",
                Title = "Validation Error",
                Status = StatusCodes.Status400BadRequest,
                Detail = "One or more validation errors occurred.",
                Extensions =
                {
                    ["errors"] = validationEx.Errors
                        .GroupBy(e => e.PropertyName)
                        .ToDictionary(g => g.Key, g => g.Select(e => e.ErrorMessage).ToArray()),
                    ["traceId"] = context.TraceIdentifier
                }
            },
            KeyNotFoundException => new ProblemDetails
            {
                Type = $"https://httpstatuses.com/404",
                Title = "Resource Not Found",
                Status = StatusCodes.Status404NotFound,
                Detail = "The requested resource was not found.",
                Extensions = { ["traceId"] = context.TraceIdentifier }
            },
            
            UnauthorizedAccessException => new ProblemDetails
            {
                Type = $"https://httpstatuses.com/401",
                Title = "Unauthorized",
                Status = StatusCodes.Status401Unauthorized,
                Detail = "Authentication is required to access this resource.",
                Extensions = { ["traceId"] = context.TraceIdentifier }
            },
            
            NotImplementedException => new ProblemDetails
            {
                Type = $"https://httpstatuses.com/501",
                Title = "Not Implemented",
                Status = StatusCodes.Status501NotImplemented,
                Detail = "This feature is not implemented yet.",
                Extensions = { ["traceId"] = context.TraceIdentifier }
            },
            ArgumentException argEx => new ProblemDetails
            {
                Type = $"https://httpstatuses.com/400",
                Title = "Bad Request",
                Status = StatusCodes.Status400BadRequest,
                Detail = argEx.Message,
                Extensions = { ["traceId"] = context.TraceIdentifier }
            },

            // 5. Fallback (Internal Server Error)
            _ => new ProblemDetails
            {
                Type = $"https://httpstatuses.com/500",
                Title = "Internal Server Error",
                Status = StatusCodes.Status500InternalServerError,
                Detail = "An unexpected error has occurred. Please try again later.",
                Extensions = { ["traceId"] = context.TraceIdentifier }
            }
        };

        context.Response.StatusCode = problemDetails.Status ?? (int)HttpStatusCode.InternalServerError;

        var json = JsonSerializer.Serialize(problemDetails, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        });

        await context.Response.WriteAsync(json);
    }
}
