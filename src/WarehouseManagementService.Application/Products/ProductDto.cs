using WarehouseManagementService.Domain.Enums;

namespace WarehouseManagementService.Application.Products;

public sealed record ProductDto(
    int Id,
    string Name,
    string Sku,
    int CategoryId,
    string CategoryName,
    ProductStatus Status);
