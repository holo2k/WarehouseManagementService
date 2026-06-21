using WarehouseManagementService.Application.Products;
using WarehouseManagementService.Application.Products.Commands.ChangeProductStatus;
using WarehouseManagementService.Application.Products.Commands.CreateProduct;
using WarehouseManagementService.Domain.Enums;

namespace WarehouseManagementService.Tests;

public sealed class ProductRequestValidatorTests
{
    [Fact]
    public void CreateProductValidator_Returns_Field_Errors_For_Invalid_Request()
    {
        var validator = new CreateProductCommandValidator();
        var request = new CreateProductRequest(
            new string('1', 201),
            new string('S', 65),
            0,
            "qqq");

        var result = validator.Validate(new CreateProductCommand(request));

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, error => error.PropertyName == nameof(CreateProductRequest.Name));
        Assert.Contains(result.Errors, error => error.PropertyName == nameof(CreateProductRequest.Sku));
        Assert.Contains(result.Errors, error => error.PropertyName == nameof(CreateProductRequest.CategoryId));
        Assert.Contains(result.Errors, error => error.PropertyName == nameof(CreateProductRequest.Status));
    }

    [Fact]
    public void ChangeProductStatusValidator_Returns_Status_Error_For_Invalid_Status()
    {
        var validator = new ChangeProductStatusCommandValidator();
        var request = new ChangeProductStatusRequest("qqq");

        var result = validator.Validate(new ChangeProductStatusCommand(2, request));

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, error => error.PropertyName == nameof(ChangeProductStatusRequest.Status));
    }

    [Fact]
    public void ProductStatusParser_Parses_Status_Case_Insensitive()
    {
        var result = ProductStatusParser.TryParse(" defective ", out var status);

        Assert.True(result);
        Assert.Equal(ProductStatus.Defective, status);
    }

    [Fact]
    public void ProductStatusParser_ParseOrDefault_Uses_Default_Only_When_Status_Is_Not_Provided()
    {
        var defaultStatus = ProductStatus.Active;

        var result = ProductStatusParser.ParseOrDefault(null, defaultStatus);

        Assert.Equal(defaultStatus, result);
        Assert.Throws<InvalidOperationException>(() => ProductStatusParser.ParseOrDefault("qqq", defaultStatus));
    }
}
