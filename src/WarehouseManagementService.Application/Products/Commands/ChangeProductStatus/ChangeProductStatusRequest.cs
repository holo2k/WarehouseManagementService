using WarehouseManagementService.Domain.Enums;

namespace WarehouseManagementService.Application.Products.Commands.ChangeProductStatus;

public sealed record ChangeProductStatusRequest(ProductStatus Status);
