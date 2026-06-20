namespace WarehouseManagementService.Application.Common.Exceptions;

public sealed class ValidationException : Exception
{
    public ValidationException(
        IReadOnlyDictionary<string, string[]> errors,
        string message = "Validation failed.")
        : base(message)
    {
        Errors = errors;
    }

    public IReadOnlyDictionary<string, string[]> Errors { get; }
}
