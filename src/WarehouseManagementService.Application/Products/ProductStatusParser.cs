using WarehouseManagementService.Domain.Enums;

namespace WarehouseManagementService.Application.Products;

public static class ProductStatusParser
{
    public const string AllowedValues = "Active, Defective, WriteOff";

    public static bool TryParse(string? value, out ProductStatus status)
    {
        return Enum.TryParse(value?.Trim(), ignoreCase: true, out status)
            && Enum.IsDefined(status);
    }

    public static ProductStatus ParseOrDefault(string? value, ProductStatus defaultStatus)
    {
        return value is null
            ? defaultStatus
            : Parse(value);
    }

    public static ProductStatus Parse(string? value)
    {
        if (TryParse(value, out var status))
        {
            return status;
        }

        throw new InvalidOperationException($"Status must be one of: {AllowedValues}.");
    }
}
