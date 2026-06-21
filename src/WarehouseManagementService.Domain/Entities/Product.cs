using WarehouseManagementService.Domain.Enums;

namespace WarehouseManagementService.Domain.Entities;

public sealed class Product
{
    private Product()
    {
    }

    public Product(string name, string sku, int categoryId, ProductStatus status)
    {
        Name = name.Trim();
        Sku = NormalizeSku(sku);
        CategoryId = categoryId;
        Status = status;
    }

    public int Id { get; private set; }

    public string Name { get; private set; } = string.Empty;

    public string Sku { get; private set; } = string.Empty;

    public int CategoryId { get; private set; }

    public Category? Category { get; private set; }

    public ProductStatus Status { get; private set; }

    public static string NormalizeSku(string sku)
    {
        return sku.Trim().ToUpperInvariant();
    }

    public bool CanChangeStatus(ProductStatus newStatus)
    {
        return Status == newStatus
            || Status == ProductStatus.Active && newStatus == ProductStatus.Defective
            || Status == ProductStatus.Defective && newStatus == ProductStatus.WriteOff;
    }

    public void ChangeStatus(ProductStatus newStatus)
    {
        if (Status == newStatus)
        {
            return;
        }

        if (!CanChangeStatus(newStatus))
        {
            throw new InvalidOperationException(
                $"Status transition from {Status} to {newStatus} is not allowed.");
        }

        Status = newStatus;
    }
}
