using AutoMapper;
using AutoMapper.QueryableExtensions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using WarehouseManagementService.Application.Common.Exceptions;
using WarehouseManagementService.Application.Common.Interfaces;
using WarehouseManagementService.Application.Common.Models;
using WarehouseManagementService.Domain.Entities;

namespace WarehouseManagementService.Application.Products.Commands.CreateProduct;

public sealed class CreateProductCommandHandler : IRequestHandler<CreateProductCommand, Result<ProductDto>>
{
    private readonly IAppDbContext _dbContext;
    private readonly IMapper _mapper;

    public CreateProductCommandHandler(IAppDbContext dbContext, IMapper mapper)
    {
        _dbContext = dbContext;
        _mapper = mapper;
    }

    public async Task<Result<ProductDto>> Handle(
        CreateProductCommand request,
        CancellationToken cancellationToken)
    {
        var productRequest = request.Request;
        var normalizedSku = productRequest.Sku.Trim();
        var skuExists = await _dbContext.Products
            .AnyAsync(product => product.Sku == normalizedSku, cancellationToken);

        if (skuExists)
        {
            throw new ConflictException($"Product with SKU '{normalizedSku}' already exists.");
        }

        var categoryExists = await _dbContext.Categories
            .AnyAsync(category => category.Id == productRequest.CategoryId, cancellationToken);

        if (!categoryExists)
        {
            throw new NotFoundException($"Category with id '{productRequest.CategoryId}' was not found.");
        }

        var product = new Product(productRequest.Name, normalizedSku, productRequest.CategoryId, productRequest.Status);
        _dbContext.Products.Add(product);
        await _dbContext.SaveChangesAsync(cancellationToken);

        var dto = await _dbContext.Products
            .AsNoTracking()
            .Where(createdProduct => createdProduct.Id == product.Id)
            .ProjectTo<ProductDto>(_mapper.ConfigurationProvider)
            .SingleAsync(cancellationToken);

        return Result<ProductDto>.Success(dto);
    }
}
