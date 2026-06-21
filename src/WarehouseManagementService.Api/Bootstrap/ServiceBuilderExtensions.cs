using System.Reflection;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Mvc;
using WarehouseManagementService.Application;
using WarehouseManagementService.Application.Common.Models;
using WarehouseManagementService.Infrastructure;

namespace WarehouseManagementService.Api.Bootstrap;

public static class ServiceBuilderExtensions
{
    public static IServiceCollection AddServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services
            .AddControllers()
            .AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
            });

        services.Configure<ApiBehaviorOptions>(options =>
        {
            options.InvalidModelStateResponseFactory = context =>
            {
                var errorEntries = context.ModelState
                    .Where(entry => entry.Value?.Errors.Count > 0)
                    .ToArray();
                var hasJsonPathErrors = errorEntries.Any(entry => entry.Key.StartsWith("$."));

                var errors = errorEntries
                    .Where(entry => !ShouldSkipRequestLevelError(entry.Key, hasJsonPathErrors))
                    .GroupBy(entry => NormalizeModelStateKey(entry.Key))
                    .ToDictionary(
                        group => group.Key,
                        group => group.SelectMany(entry => entry.Value!.Errors)
                            .Select(error => NormalizeModelStateErrorMessage(error.ErrorMessage))
                            .Distinct()
                            .ToArray());

                var response = ApiResponse<object>.Fail(
                    new ErrorResponse(ErrorCodes.Validation, "Validation failed.", errors));

                return new UnprocessableEntityObjectResult(response);
            };
        });

        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen(options =>
        {
            var xmlFileName = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
            var xmlFilePath = Path.Combine(AppContext.BaseDirectory, xmlFileName);
            options.IncludeXmlComments(xmlFilePath);
        });

        services.AddApplication();
        services.AddInfrastructure(configuration);

        return services;
    }

    private static bool ShouldSkipRequestLevelError(string key, bool hasJsonPathErrors)
    {
        return hasJsonPathErrors
            && string.Equals(key, "request", StringComparison.OrdinalIgnoreCase);
    }

    private static string NormalizeModelStateKey(string key)
    {
        if (string.IsNullOrWhiteSpace(key) || key == "$")
        {
            return "request";
        }

        return key.StartsWith("$.", StringComparison.Ordinal)
            ? key[2..]
            : key;
    }

    private static string NormalizeModelStateErrorMessage(string message)
    {
        if (string.IsNullOrWhiteSpace(message))
        {
            return "Invalid value.";
        }

        if (message.Contains("could not be converted", StringComparison.OrdinalIgnoreCase))
        {
            return "Invalid value.";
        }

        return string.Equals(message, "The request field is required.", StringComparison.OrdinalIgnoreCase)
            ? "Request body is required."
            : message;
    }
}
