using WarehouseManagementService.Api.Middleware;
using WarehouseManagementService.Application.Common.Models;
using WarehouseManagementService.Infrastructure.Persistence;
using WarehouseManagementService.Infrastructure.Persistence.Initializer;

namespace WarehouseManagementService.Api.Bootstrap;

public static class ApplicationBuilderExtensions
{
    public static WebApplication UseWebApi(this WebApplication app)
    {
        app.UseMiddleware<ExceptionHandlingMiddleware>();
        app.UseJsonStatusCodePages();

        app.UseSwagger();
        app.UseSwaggerUI();

        app.MapControllers();
        app.MapGet("/health", () => Results.Ok(ApiResponse<object>.Ok(new { status = "ok" })));

        return app;
    }

    public static async Task InitializeDatabaseAsync(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        await DbInitializer.InitializeAsync(dbContext);
    }

    private static void UseJsonStatusCodePages(this WebApplication app)
    {
        app.UseStatusCodePages(async statusCodeContext =>
        {
            var httpContext = statusCodeContext.HttpContext;

            if (httpContext.Response.HasStarted)
            {
                return;
            }

            var statusCode = httpContext.Response.StatusCode;
            var error = statusCode switch
            {
                StatusCodes.Status404NotFound => new ErrorResponse(
                    ErrorCodes.NotFound,
                    "The requested resource was not found."),

                StatusCodes.Status405MethodNotAllowed => new ErrorResponse(
                    ErrorCodes.MethodNotAllowed,
                    "The HTTP method is not allowed for this endpoint."),

                _ => new ErrorResponse(
                    ErrorCodes.HttpError,
                    $"Request failed with HTTP status code {statusCode}.")
            };

            httpContext.Response.ContentType = "application/json";
            await httpContext.Response.WriteAsJsonAsync(ApiResponse<object>.Fail(error));
        });
    }
}
