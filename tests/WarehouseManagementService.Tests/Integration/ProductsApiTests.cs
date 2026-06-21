using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using WarehouseManagementService.Application.Categories;
using WarehouseManagementService.Application.Common.Models;
using WarehouseManagementService.Application.Products;
using WarehouseManagementService.Application.Products.Commands.ChangeProductStatus;
using WarehouseManagementService.Application.Products.Commands.CreateProduct;
using WarehouseManagementService.Domain.Enums;

namespace WarehouseManagementService.Tests.Integration;

public sealed class ProductsApiTests : IClassFixture<WarehouseApiFactory>
{
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web)
    {
        Converters = { new JsonStringEnumConverter() }
    };

    private readonly HttpClient _client;

    public ProductsApiTests(WarehouseApiFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task GetCategories_Returns_Seeded_Categories()
    {
        var response = await _client.GetAsync("/api/categories");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var body = await ReadApiResponseAsync<IReadOnlyCollection<CategoryDto>>(response);
        Assert.True(body.Success);
        Assert.NotNull(body.Data);
        Assert.True(body.Data.Count >= 3);
    }

    [Fact]
    public async Task CreateProduct_Returns_Created_Product()
    {
        var category = await GetFirstCategoryAsync();
        var request = new CreateProductRequest(
            "Integration Test Product",
            CreateSku(),
            category.Id,
            nameof(ProductStatus.Active));

        var response = await _client.PostAsJsonAsync("/api/products", request, JsonOptions);

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);

        var body = await ReadApiResponseAsync<ProductDto>(response);
        Assert.True(body.Success);
        Assert.NotNull(body.Data);
        Assert.Equal(request.Name, body.Data.Name);
        Assert.Equal(request.Sku, body.Data.Sku);
        Assert.Equal(ProductStatus.Active, body.Data.Status);
    }

    [Fact]
    public async Task CreateProduct_With_Duplicate_Sku_Returns_Conflict()
    {
        var category = await GetFirstCategoryAsync();
        var sku = CreateSku();
        var request = new CreateProductRequest(
            "Duplicate SKU Product",
            sku,
            category.Id,
            nameof(ProductStatus.Active));

        var firstResponse = await _client.PostAsJsonAsync("/api/products", request, JsonOptions);
        var secondResponse = await _client.PostAsJsonAsync("/api/products", request, JsonOptions);

        Assert.Equal(HttpStatusCode.Created, firstResponse.StatusCode);
        Assert.Equal(HttpStatusCode.Conflict, secondResponse.StatusCode);

        var body = await ReadApiResponseAsync<ProductDto>(secondResponse);
        Assert.False(body.Success);
        Assert.Equal(ErrorCodes.Conflict, body.Error?.Code);
    }

    [Fact]
    public async Task ChangeProductStatus_From_Active_To_WriteOff_Returns_Domain_Error()
    {
        var product = await CreateProductAsync(nameof(ProductStatus.Active));
        var request = new ChangeProductStatusRequest(nameof(ProductStatus.WriteOff));

        var response = await _client.PatchAsJsonAsync(
            $"/api/products/{product.Id}/status",
            request,
            JsonOptions);

        Assert.Equal(HttpStatusCode.UnprocessableEntity, response.StatusCode);

        var body = await ReadApiResponseAsync<ProductDto>(response);
        Assert.False(body.Success);
        Assert.Equal(ErrorCodes.DomainRuleViolation, body.Error?.Code);
    }

    [Fact]
    public async Task ChangeProductStatus_With_Invalid_Status_Returns_Validation_Error()
    {
        var product = await CreateProductAsync(nameof(ProductStatus.Active));
        var request = new ChangeProductStatusRequest("qqq");

        var response = await _client.PatchAsJsonAsync(
            $"/api/products/{product.Id}/status",
            request,
            JsonOptions);

        Assert.Equal(HttpStatusCode.UnprocessableEntity, response.StatusCode);

        var body = await ReadApiResponseAsync<ProductDto>(response);
        Assert.False(body.Success);
        Assert.Equal(ErrorCodes.Validation, body.Error?.Code);
        Assert.True(body.Error?.Details?.ContainsKey(nameof(ChangeProductStatusRequest.Status)));
    }

    private async Task<ProductDto> CreateProductAsync(string status)
    {
        var category = await GetFirstCategoryAsync();
        var request = new CreateProductRequest(
            "Status Test Product",
            CreateSku(),
            category.Id,
            status);

        var response = await _client.PostAsJsonAsync("/api/products", request, JsonOptions);

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);

        var body = await ReadApiResponseAsync<ProductDto>(response);
        Assert.True(body.Success);
        Assert.NotNull(body.Data);

        return body.Data;
    }

    private async Task<CategoryDto> GetFirstCategoryAsync()
    {
        var response = await _client.GetAsync("/api/categories");
        var body = await ReadApiResponseAsync<IReadOnlyCollection<CategoryDto>>(response);

        Assert.NotNull(body.Data);

        return body.Data.First();
    }

    private static async Task<ApiResponse<T>> ReadApiResponseAsync<T>(HttpResponseMessage response)
    {
        var body = await response.Content.ReadFromJsonAsync<ApiResponse<T>>(JsonOptions);

        Assert.NotNull(body);

        return body;
    }

    private static string CreateSku()
    {
        return $"IT-{Guid.NewGuid():N}"[..32].ToUpperInvariant();
    }
}
