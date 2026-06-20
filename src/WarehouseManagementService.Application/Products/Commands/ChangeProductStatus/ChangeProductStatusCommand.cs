using MediatR;
using WarehouseManagementService.Application.Common.Models;
using WarehouseManagementService.Domain.Enums;

namespace WarehouseManagementService.Application.Products.Commands.ChangeProductStatus;

public sealed record ChangeProductStatusCommand(
    int Id,
    ChangeProductStatusRequest Request) : IRequest<Result<ProductDto>>;
