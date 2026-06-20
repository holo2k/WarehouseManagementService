using MediatR;
using WarehouseManagementService.Application.Common.Models;

namespace WarehouseManagementService.Application.Categories.Commands.CreateCategory;

public sealed record CreateCategoryCommand(CreateCategoryRequest Request) : IRequest<Result<CategoryDto>>;
