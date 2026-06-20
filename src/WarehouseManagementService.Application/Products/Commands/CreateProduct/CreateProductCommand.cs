using MediatR;
using WarehouseManagementService.Application.Common.Models;

namespace WarehouseManagementService.Application.Products.Commands.CreateProduct;

public sealed record CreateProductCommand(
    CreateProductRequest Request) : IRequest<Result<ProductDto>>;
