namespace WarehouseManagementService.Application.Common.Models;

public sealed class Result<T>
{
    private readonly T? _value;

    private Result(T value)
    {
        _value = value;
    }

    private Result(ErrorResponse error)
    {
        ArgumentNullException.ThrowIfNull(error);

        Error = error;
    }

    public bool IsSuccess => Error is null;

    public bool IsFailure => !IsSuccess;

    public T Value => IsSuccess
        ? _value!
        : throw new InvalidOperationException("Cannot access value of failed result.");

    public ErrorResponse? Error { get; }

    public static Result<T> Success(T value) => new(value);

    public static Result<T> Failure(ErrorResponse error) => new(error);

    public static Result<T> Failure(
        string code,
        string message,
        IReadOnlyDictionary<string, string[]>? details = null) =>
        new(new ErrorResponse(code, message, details));
}

public static class Result
{
    public static Result<T> Success<T>(T value) => Result<T>.Success(value);

    public static Result<T> Failure<T>(ErrorResponse error) => Result<T>.Failure(error);

    public static Result<T> Failure<T>(
        string code,
        string message,
        IReadOnlyDictionary<string, string[]>? details = null) =>
        Result<T>.Failure(code, message, details);

    public static object Failure(Type valueType, ErrorResponse error)
    {
        ArgumentNullException.ThrowIfNull(valueType);
        ArgumentNullException.ThrowIfNull(error);

        var resultType = typeof(Result<>).MakeGenericType(valueType);
        var method = resultType.GetMethod(
            nameof(Result<object>.Failure),
            [typeof(ErrorResponse)]);

        return method!.Invoke(null, [error])!;
    }
}
