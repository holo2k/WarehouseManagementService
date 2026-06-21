using AutoMapper;
using MediatR;
using WarehouseManagementService.Application.Common.Interfaces;
using WarehouseManagementService.Application.Common.Models;

namespace WarehouseManagementService.Application.Products.Queries.GetProductById;

public sealed class GetProductByIdQueryHandler : IRequestHandler<GetProductByIdQuery, Result<ProductDto>>
{
    private readonly IProductRepository _productRepository;
    private readonly IMapper _mapper;

    public GetProductByIdQueryHandler(IProductRepository productRepository, IMapper mapper)
    {
        _productRepository = productRepository;
        _mapper = mapper;
    }

    public async Task<Result<ProductDto>> Handle(
        GetProductByIdQuery request,
        CancellationToken cancellationToken)
    {
        var product = await _productRepository.GetByIdWithCategoryAsync(
            request.Id,
            trackChanges: false,
            cancellationToken);

        if (product is null)
        {
            return Result.Failure<ProductDto>(
                ErrorCodes.NotFound,
                $"Product with id '{request.Id}' was not found.");
        }

        return Result.Success(_mapper.Map<ProductDto>(product));
    }
}
