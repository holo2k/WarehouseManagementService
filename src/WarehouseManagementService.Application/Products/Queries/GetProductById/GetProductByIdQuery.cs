using MediatR;
using WarehouseManagementService.Application.Common.Models;
using WarehouseManagementService.Application.Products;

namespace WarehouseManagementService.Application.Products.Queries.GetProductById;

public sealed record GetProductByIdQuery(int Id) : IRequest<Result<ProductDto>>;
