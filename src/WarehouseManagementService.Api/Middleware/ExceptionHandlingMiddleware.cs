using System.Net;
using Microsoft.EntityFrameworkCore;
using WarehouseManagementService.Application.Common.Models;

namespace WarehouseManagementService.Api.Middleware;

public sealed class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    public ExceptionHandlingMiddleware(
        RequestDelegate next,
        ILogger<ExceptionHandlingMiddleware> logger)
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
        catch (Exception exception)
        {
            await HandleExceptionAsync(context, exception);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        var (statusCode, error) = exception switch
        {
            DbUpdateConcurrencyException => (
                HttpStatusCode.Conflict,
                new ErrorResponse(ErrorCodes.ConcurrencyConflict, "The product was changed by another request. Reload it and retry.")),

            DbUpdateException => (
                HttpStatusCode.Conflict,
                new ErrorResponse(ErrorCodes.DatabaseConflict, "The database rejected the change. Check unique fields and references.")),

            _ => (
                HttpStatusCode.InternalServerError,
                new ErrorResponse(ErrorCodes.InternalServerError, "An unexpected error occurred."))
        };

        if (statusCode == HttpStatusCode.InternalServerError)
        {
            _logger.LogError(exception, "Unhandled exception");
        }
        else
        {
            _logger.LogWarning(exception, "Request failed with handled exception");
        }

        context.Response.StatusCode = (int)statusCode;
        context.Response.ContentType = "application/json";

        await context.Response.WriteAsJsonAsync(ApiResponse<object>.Fail(error));
    }
}
