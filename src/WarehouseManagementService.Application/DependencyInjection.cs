using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using WarehouseManagementService.Application.Common.Behaviors;

namespace WarehouseManagementService.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        var assembly = typeof(DependencyInjection).Assembly;

        services.AddMediatR(configuration =>
            configuration.RegisterServicesFromAssembly(assembly));

        services.AddAutoMapper(configuration => configuration.AddMaps(assembly));
        services.AddValidatorsFromAssembly(assembly);
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));

        return services;
    }
}
