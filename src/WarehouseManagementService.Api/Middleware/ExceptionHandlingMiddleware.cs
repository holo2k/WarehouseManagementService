using System.Net;
using Microsoft.EntityFrameworkCore;
using WarehouseManagementService.Application.Common.Exceptions;
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
            ValidationException validationException => (
                HttpStatusCode.BadRequest,
                new ErrorResponse("validation_error", validationException.Message, validationException.Errors)),

            NotFoundException notFoundException => (
                HttpStatusCode.NotFound,
                new ErrorResponse("not_found", notFoundException.Message)),

            ConflictException conflictException => (
                HttpStatusCode.Conflict,
                new ErrorResponse("conflict", conflictException.Message)),

            DomainRuleException domainRuleException => (
                HttpStatusCode.UnprocessableEntity,
                new ErrorResponse("domain_rule_violation", domainRuleException.Message)),

            DbUpdateConcurrencyException => (
                HttpStatusCode.Conflict,
                new ErrorResponse("concurrency_conflict", "The product was changed by another request. Reload it and retry.")),

            DbUpdateException => (
                HttpStatusCode.Conflict,
                new ErrorResponse("database_conflict", "The database rejected the change. Check unique fields and references.")),

            _ => (
                HttpStatusCode.InternalServerError,
                new ErrorResponse("internal_server_error", "An unexpected error occurred."))
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
