using AutoMapper;
using MediatR;
using WarehouseManagementService.Application.Common.Interfaces;
using WarehouseManagementService.Application.Common.Models;

namespace WarehouseManagementService.Application.Products.Commands.ChangeProductStatus;

public sealed class ChangeProductStatusCommandHandler : IRequestHandler<ChangeProductStatusCommand, Result<ProductDto>>
{
    private readonly IProductRepository _productRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public ChangeProductStatusCommandHandler(
        IProductRepository productRepository,
        IUnitOfWork unitOfWork,
        IMapper mapper)
    {
        _productRepository = productRepository;
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<Result<ProductDto>> Handle(
        ChangeProductStatusCommand request,
        CancellationToken cancellationToken)
    {
        var product = await _productRepository.GetByIdWithCategoryAsync(
            request.Id,
            trackChanges: true,
            cancellationToken);

        if (product is null)
        {
            return Result.Failure<ProductDto>(
                ErrorCodes.NotFound,
                $"Product with id '{request.Id}' was not found.");
        }

        if (!product.CanChangeStatus(request.Request.Status))
        {
            return Result.Failure<ProductDto>(
                ErrorCodes.DomainRuleViolation,
                $"Status transition from {product.Status} to {request.Request.Status} is not allowed.");
        }

        product.ChangeStatus(request.Request.Status);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success(_mapper.Map<ProductDto>(product));
    }
}
