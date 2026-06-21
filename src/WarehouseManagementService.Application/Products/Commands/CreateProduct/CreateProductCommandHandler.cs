using AutoMapper;
using MediatR;
using WarehouseManagementService.Application.Common.Interfaces;
using WarehouseManagementService.Application.Common.Models;
using WarehouseManagementService.Domain.Enums;
using WarehouseManagementService.Domain.Entities;

namespace WarehouseManagementService.Application.Products.Commands.CreateProduct;

public sealed class CreateProductCommandHandler : IRequestHandler<CreateProductCommand, Result<ProductDto>>
{
    private readonly IProductRepository _productRepository;
    private readonly ICategoryRepository _categoryRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public CreateProductCommandHandler(
        IProductRepository productRepository,
        ICategoryRepository categoryRepository,
        IUnitOfWork unitOfWork,
        IMapper mapper)
    {
        _productRepository = productRepository;
        _categoryRepository = categoryRepository;
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<Result<ProductDto>> Handle(
        CreateProductCommand request,
        CancellationToken cancellationToken)
    {
        var productRequest = request.Request;
        var normalizedSku = Product.NormalizeSku(productRequest.Sku);
        var skuExists = await _productRepository.ExistsBySkuAsync(normalizedSku, cancellationToken);

        if (skuExists)
        {
            return Result.Failure<ProductDto>(
                ErrorCodes.Conflict,
                $"Product with SKU '{normalizedSku}' already exists.");
        }

        var categoryExists = await _categoryRepository.ExistsByIdAsync(productRequest.CategoryId, cancellationToken);

        if (!categoryExists)
        {
            return Result.Failure<ProductDto>(
                ErrorCodes.NotFound,
                $"Category with id '{productRequest.CategoryId}' was not found.");
        }

        var status = ProductStatusParser.ParseOrDefault(productRequest.Status, ProductStatus.Active);
        var product = new Product(productRequest.Name, normalizedSku, productRequest.CategoryId, status);
        _productRepository.Add(product);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        var createdProduct = await _productRepository.GetByIdWithCategoryAsync(
            product.Id,
            trackChanges: false,
            cancellationToken);

        return Result.Success(_mapper.Map<ProductDto>(createdProduct ?? product));
    }
}
