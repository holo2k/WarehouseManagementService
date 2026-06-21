using AutoMapper;
using MediatR;
using WarehouseManagementService.Application.Common.Interfaces;
using WarehouseManagementService.Application.Common.Models;
using WarehouseManagementService.Domain.Entities;

namespace WarehouseManagementService.Application.Categories.Commands.CreateCategory;

public sealed class CreateCategoryCommandHandler : IRequestHandler<CreateCategoryCommand, Result<CategoryDto>>
{
    private readonly ICategoryRepository _categoryRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public CreateCategoryCommandHandler(
        ICategoryRepository categoryRepository,
        IUnitOfWork unitOfWork,
        IMapper mapper)
    {
        _categoryRepository = categoryRepository;
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<Result<CategoryDto>> Handle(
        CreateCategoryCommand request,
        CancellationToken cancellationToken)
    {
        var normalizedName = request.Request.Name.Trim();
        var exists = await _categoryRepository.ExistsByNameAsync(normalizedName, cancellationToken);

        if (exists)
        {
            return Result.Failure<CategoryDto>(
                ErrorCodes.Conflict,
                $"Category '{normalizedName}' already exists.");
        }

        var category = new Category(normalizedName);
        _categoryRepository.Add(category);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success(_mapper.Map<CategoryDto>(category));
    }
}
