using WarehouseManagementService.Domain.Enums;

namespace WarehouseManagementService.Application.Products.Commands.CreateProduct;

public sealed record CreateProductRequest(
    string Name,
    string Sku,
    int CategoryId,
    ProductStatus Status = ProductStatus.Active);
