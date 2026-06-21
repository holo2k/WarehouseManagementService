using AutoMapper;
using MediatR;
using WarehouseManagementService.Application.Common.Interfaces;
using WarehouseManagementService.Application.Common.Models;

namespace WarehouseManagementService.Application.Products.Queries.GetProducts;

public sealed class GetProductsQueryHandler : IRequestHandler<GetProductsQuery, Result<PagedResult<ProductDto>>>
{
    private readonly IProductRepository _productRepository;
    private readonly IMapper _mapper;

    public GetProductsQueryHandler(IProductRepository productRepository, IMapper mapper)
    {
        _productRepository = productRepository;
        _mapper = mapper;
    }

    public async Task<Result<PagedResult<ProductDto>>> Handle(
        GetProductsQuery request,
        CancellationToken cancellationToken)
    {
        var totalCount = await _productRepository.CountAsync(
            request.Status,
            request.CategoryId,
            cancellationToken);

        var products = await _productRepository.GetPageAsync(
            request.Status,
            request.CategoryId,
            request.Page,
            request.PageSize,
            cancellationToken);
        var productDtos = _mapper.Map<IReadOnlyCollection<ProductDto>>(products);

        return Result.Success(
            new PagedResult<ProductDto>(
                productDtos,
                request.Page,
                request.PageSize,
                totalCount));
    }
}
