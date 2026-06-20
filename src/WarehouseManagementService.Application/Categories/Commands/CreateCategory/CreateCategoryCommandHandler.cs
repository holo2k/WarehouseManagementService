using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using WarehouseManagementService.Application.Common.Exceptions;
using WarehouseManagementService.Application.Common.Interfaces;
using WarehouseManagementService.Application.Common.Models;
using WarehouseManagementService.Domain.Entities;

namespace WarehouseManagementService.Application.Categories.Commands.CreateCategory;

public sealed class CreateCategoryCommandHandler : IRequestHandler<CreateCategoryCommand, Result<CategoryDto>>
{
    private readonly IAppDbContext _dbContext;
    private readonly IMapper _mapper;

    public CreateCategoryCommandHandler(IAppDbContext dbContext, IMapper mapper)
    {
        _dbContext = dbContext;
        _mapper = mapper;
    }

    public async Task<Result<CategoryDto>> Handle(
        CreateCategoryCommand request,
        CancellationToken cancellationToken)
    {
        var normalizedName = request.Request.Name.Trim();
        var exists = await _dbContext.Categories
            .AnyAsync(category => category.Name == normalizedName, cancellationToken);

        if (exists)
        {
            throw new ConflictException($"Category '{normalizedName}' already exists.");
        }

        var category = new Category(normalizedName);
        _dbContext.Categories.Add(category);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return Result<CategoryDto>.Success(_mapper.Map<CategoryDto>(category));
    }
}
