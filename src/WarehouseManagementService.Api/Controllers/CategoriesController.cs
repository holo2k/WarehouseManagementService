using MediatR;
using Microsoft.AspNetCore.Mvc;
using WarehouseManagementService.Api.Extensions;
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

    /// <summary>
    /// Получить список всех категорий товаров.
    /// </summary>
    /// <param name="cancellationToken">Токен отмены запроса.</param>
    /// <returns>Список категорий.</returns>
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<IReadOnlyCollection<CategoryDto>>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<IReadOnlyCollection<CategoryDto>>>> GetAll(
        CancellationToken cancellationToken)
    {
        var result = await _sender.Send(new GetCategoriesQuery(), cancellationToken);

        return this.FromResult(result);
    }

    /// <summary>
    /// Создать новую категорию товара.
    /// </summary>
    /// <param name="request">Данные новой категории.</param>
    /// <param name="cancellationToken">Токен отмены запроса.</param>
    /// <returns>Созданная категория.</returns>
    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<CategoryDto>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status409Conflict)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status422UnprocessableEntity)]
    public async Task<ActionResult<ApiResponse<CategoryDto>>> Create(
        [FromBody] CreateCategoryRequest request,
        CancellationToken cancellationToken)
    {
        var result = await _sender.Send(new CreateCategoryCommand(request), cancellationToken);

        return this.FromCreatedResult(result, category => $"/api/categories/{category.Id}");
    }
}
