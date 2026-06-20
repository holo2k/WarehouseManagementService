namespace WarehouseManagementService.Application.Common.Models;

public sealed record ApiResponse<T>(
    bool Success,
    T? Data,
    ErrorResponse? Error)
{
    public static ApiResponse<T> Ok(T data) => new(true, data, null);

    public static ApiResponse<T> Fail(ErrorResponse error) => new(false, default, error);
}

public sealed record ErrorResponse(
    string Code,
    string Message,
    IReadOnlyDictionary<string, string[]>? Details = null);
