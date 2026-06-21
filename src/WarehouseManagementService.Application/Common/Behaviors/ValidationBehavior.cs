using FluentValidation;
using MediatR;
using WarehouseManagementService.Application.Common.Models;

namespace WarehouseManagementService.Application.Common.Behaviors;

public sealed class ValidationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
{
    private readonly IReadOnlyCollection<IValidator<TRequest>> _validators;

    public ValidationBehavior(IEnumerable<IValidator<TRequest>> validators)
    {
        _validators = validators.ToArray();
    }

    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        if (_validators.Count == 0)
        {
            return await next(cancellationToken);
        }

        var context = new ValidationContext<TRequest>(request);
        var validationResults = await Task.WhenAll(
            _validators.Select(validator => validator.ValidateAsync(context, cancellationToken)));

        var errors = validationResults
            .SelectMany(result => result.Errors)
            .Where(failure => failure is not null)
            .GroupBy(failure => failure.PropertyName)
            .ToDictionary(
                group => group.Key,
                group => group.Select(failure => failure.ErrorMessage).Distinct().ToArray());

        if (errors.Count > 0)
        {
            var error = new ErrorResponse(ErrorCodes.Validation, "Validation failed.", errors);

            if (TryCreateFailureResponse(error, out TResponse response))
            {
                return response;
            }

            throw new InvalidOperationException($"{nameof(ValidationBehavior<TRequest, TResponse>)} supports only Result<T> responses.");
        }

        return await next(cancellationToken);
    }

    private static bool TryCreateFailureResponse(ErrorResponse error, out TResponse response)
    {
        var responseType = typeof(TResponse);

        if (!responseType.IsGenericType || responseType.GetGenericTypeDefinition() != typeof(Result<>))
        {
            response = default!;
            return false;
        }

        var valueType = responseType.GetGenericArguments()[0];
        response = (TResponse)Result.Failure(valueType, error);

        return true;
    }
}
