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
        Sku = sku.Trim();
        CategoryId = categoryId;
        Status = status;
    }

    public int Id { get; private set; }

    public string Name { get; private set; } = string.Empty;

    public string Sku { get; private set; } = string.Empty;

    public int CategoryId { get; private set; }

    public Category? Category { get; private set; }

    public ProductStatus Status { get; private set; }

    public void ChangeStatus(ProductStatus newStatus)
    {
        if (Status == newStatus)
        {
            return;
        }

        var isAllowed = Status == ProductStatus.Active && newStatus == ProductStatus.Defective
            || Status == ProductStatus.Defective && newStatus == ProductStatus.WriteOff;

        if (!isAllowed)
        {
            throw new InvalidOperationException(
                $"Status transition from {Status} to {newStatus} is not allowed.");
        }

        Status = newStatus;
    }
}
