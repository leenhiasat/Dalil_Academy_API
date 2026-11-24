using Dalil_Academy.Core.Exceptions;
using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Net.Http;

namespace Dalil_Academy.API.Middleware;

public class GlobalExceptionHandler(
    ILogger<GlobalExceptionHandler> logger)
    : IMiddleware
{
    public async Task InvokeAsync(
        HttpContext context,
        RequestDelegate next)
    {
        try
        {
            await next(context);
        }
        catch (ValidationException ex)
        {
            logger.LogWarning(
                ex,
                "Validation failed.");

            context.Response.StatusCode =
                StatusCodes.Status400BadRequest;

            var errors = ex.Errors
                .GroupBy(e => e.PropertyName)
                .ToDictionary(
                    group => group.Key,
                    group => group.Select(e => e.ErrorMessage).ToArray()
                );

            await context.Response.WriteAsJsonAsync(new
            {
                message = "One or more validation errors occurred.",
                errors
            });
        }
        catch (UnauthorizedAccessException ex)
        {
            logger.LogWarning(
                ex,
                "Unauthorized request.");

            context.Response.StatusCode =
                StatusCodes.Status401Unauthorized;

            await context.Response.WriteAsJsonAsync(new
            {
                message = ex.Message
            });
        }
        catch (PaymentFailedException ex)
        {
            logger.LogWarning(
                ex,
                "Payment failed.");

            context.Response.StatusCode =
                StatusCodes.Status400BadRequest;

            await context.Response.WriteAsJsonAsync(new
            {
                message = ex.Message
            });
        }
        catch (NotFoundException ex)
        {
            logger.LogWarning(
                ex,
                "Resource not found.");

            context.Response.StatusCode =
                StatusCodes.Status404NotFound;

            await context.Response.WriteAsJsonAsync(new
            {
                message = ex.Message
            });
        }
        catch (ConflictException ex)
        {
            logger.LogWarning(
                ex,
                "Resource conflict.");

            context.Response.StatusCode =
                StatusCodes.Status409Conflict;

            await context.Response.WriteAsJsonAsync(new
            {
                message = ex.Message
            });
        }
        catch (ArgumentException ex)
        {
            logger.LogWarning(
                ex,
                "Invalid argument.");

            context.Response.StatusCode =
                StatusCodes.Status400BadRequest;

            await context.Response.WriteAsJsonAsync(new
            {
                message = ex.Message
            });
        }
        catch (HttpRequestException ex)
        {
            logger.LogError(
                ex,
                "External service request failed.");

            context.Response.StatusCode =
                StatusCodes.Status503ServiceUnavailable;

            await context.Response.WriteAsJsonAsync(new
            {
                message =
                    "PayPal service is currently unavailable. Please try again later."
            });
        }
        catch (Exception ex)
        {
            logger.LogError(
                ex,
                "An unexpected error occurred.");

            context.Response.StatusCode =
                StatusCodes.Status500InternalServerError;

            await context.Response.WriteAsJsonAsync(new
            {
                message = "An unexpected error occurred."
            });
        }
    }
}