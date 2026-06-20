using AutoMapper;
using WarehouseManagementService.Application.Categories;
using WarehouseManagementService.Application.Products;
using WarehouseManagementService.Domain.Entities;

namespace WarehouseManagementService.Application.Common.Mappings;

public sealed class ApplicationMappingProfile : Profile
{
    public ApplicationMappingProfile()
    {
        CreateMap<Category, CategoryDto>();

        CreateMap<Product, ProductDto>()
            .ForCtorParam(
                nameof(ProductDto.CategoryName),
                options => options.MapFrom(product => product.Category != null ? product.Category.Name : string.Empty));
    }
}
