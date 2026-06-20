namespace WarehouseManagementService.Application.Common.Models;

public sealed class Result<T>
{
    private Result(T value)
    {
        Value = value;
    }

    public T Value { get; }

    public static Result<T> Success(T value) => new(value);
}
