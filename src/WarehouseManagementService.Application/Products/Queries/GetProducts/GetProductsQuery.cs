using MediatR;
using WarehouseManagementService.Application.Common.Models;
using WarehouseManagementService.Application.Products;
using WarehouseManagementService.Domain.Enums;

namespace WarehouseManagementService.Application.Products.Queries.GetProducts;

public sealed record GetProductsQuery(
    ProductStatus? Status,
    int? CategoryId,
    int Page = 1,
    int PageSize = 20) : IRequest<Result<PagedResult<ProductDto>>>;
