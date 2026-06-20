using MediatR;
using WarehouseManagementService.Application.Common.Models;

namespace WarehouseManagementService.Application.Categories.Queries.GetCategories;

public sealed record GetCategoriesQuery : IRequest<Result<IReadOnlyCollection<CategoryDto>>>;
