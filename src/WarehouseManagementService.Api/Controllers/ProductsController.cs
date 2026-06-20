using MediatR;
using Microsoft.AspNetCore.Mvc;
using WarehouseManagementService.Application.Common.Models;
using WarehouseManagementService.Application.Products;
using WarehouseManagementService.Application.Products.Commands.ChangeProductStatus;
using WarehouseManagementService.Application.Products.Commands.CreateProduct;
using WarehouseManagementService.Application.Products.Queries.GetProductById;
using WarehouseManagementService.Application.Products.Queries.GetProducts;
using WarehouseManagementService.Domain.Enums;

namespace WarehouseManagementService.Api.Controllers;

[ApiController]
[Route("api/products")]
public sealed class ProductsController : ControllerBase
{
    private readonly ISender _sender;

    public ProductsController(ISender sender)
    {
        _sender = sender;
    }

    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<PagedResult<ProductDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ApiResponse<PagedResult<ProductDto>>>> GetAll(
        [FromQuery] ProductStatus? status,
        [FromQuery] int? categoryId,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        CancellationToken cancellationToken = default)
    {
        var result = await _sender.Send(
            new GetProductsQuery(status, categoryId, page, pageSize),
            cancellationToken);

        return Ok(ApiResponse<PagedResult<ProductDto>>.Ok(result.Value));
    }

    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(ApiResponse<ProductDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse<ProductDto>>> GetById(
        int id,
        CancellationToken cancellationToken)
    {
        var result = await _sender.Send(new GetProductByIdQuery(id), cancellationToken);

        return Ok(ApiResponse<ProductDto>.Ok(result.Value));
    }

    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<ProductDto>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status409Conflict)]
    public async Task<ActionResult<ApiResponse<ProductDto>>> Create(
        CreateProductRequest request,
        CancellationToken cancellationToken)
    {
        var result = await _sender.Send(new CreateProductCommand(request), cancellationToken);

        return CreatedAtAction(nameof(GetById), new { id = result.Value.Id }, ApiResponse<ProductDto>.Ok(result.Value));
    }

    [HttpPatch("{id:int}/status")]
    [ProducesResponseType(typeof(ApiResponse<ProductDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status409Conflict)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status422UnprocessableEntity)]
    public async Task<ActionResult<ApiResponse<ProductDto>>> ChangeStatus(
        int id,
        ChangeProductStatusRequest request,
        CancellationToken cancellationToken)
    {
        var result = await _sender.Send(
            new ChangeProductStatusCommand(id, request),
            cancellationToken);

        return Ok(ApiResponse<ProductDto>.Ok(result.Value));
    }
}
