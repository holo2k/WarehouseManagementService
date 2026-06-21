using AutoMapper;
using MediatR;
using WarehouseManagementService.Application.Common.Interfaces;
using WarehouseManagementService.Application.Common.Models;

namespace WarehouseManagementService.Application.Categories.Queries.GetCategories;

public sealed class GetCategoriesQueryHandler : IRequestHandler<GetCategoriesQuery, Result<IReadOnlyCollection<CategoryDto>>>
{
    private readonly ICategoryRepository _categoryRepository;
    private readonly IMapper _mapper;

    public GetCategoriesQueryHandler(ICategoryRepository categoryRepository, IMapper mapper)
    {
        _categoryRepository = categoryRepository;
        _mapper = mapper;
    }

    public async Task<Result<IReadOnlyCollection<CategoryDto>>> Handle(
        GetCategoriesQuery request,
        CancellationToken cancellationToken)
    {
        var categories = await _categoryRepository.GetAllAsync(cancellationToken);
        var categoryDtos = _mapper.Map<IReadOnlyCollection<CategoryDto>>(categories);

        return Result.Success(categoryDtos);
    }
}
