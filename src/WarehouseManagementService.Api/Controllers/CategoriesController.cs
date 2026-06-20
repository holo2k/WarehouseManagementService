using MediatR;
using Microsoft.AspNetCore.Mvc;
using WarehouseManagementService.Application.Categories;
using WarehouseManagementService.Application.Categories.Commands.CreateCategory;
using WarehouseManagementService.Application.Categories.Queries.GetCategories;
using WarehouseManagementService.Application.Common.Models;

namespace WarehouseManagementService.Api.Controllers;

[ApiController]
[Route("api/categories")]
public sealed class CategoriesController : ControllerBase
{
    private readonly ISender _sender;

    public CategoriesController(ISender sender)
    {
        _sender = sender;
    }

    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<IReadOnlyCollection<CategoryDto>>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<IReadOnlyCollection<CategoryDto>>>> GetAll(
        CancellationToken cancellationToken)
    {
        var result = await _sender.Send(new GetCategoriesQuery(), cancellationToken);

        return Ok(ApiResponse<IReadOnlyCollection<CategoryDto>>.Ok(result.Value));
    }

    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<CategoryDto>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status409Conflict)]
    public async Task<ActionResult<ApiResponse<CategoryDto>>> Create(
        CreateCategoryRequest request,
        CancellationToken cancellationToken)
    {
        var result = await _sender.Send(new CreateCategoryCommand(request), cancellationToken);

        return Created($"/api/categories/{result.Value.Id}", ApiResponse<CategoryDto>.Ok(result.Value));
    }
}
