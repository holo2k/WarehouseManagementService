using WarehouseManagementService.Domain.Entities;
using WarehouseManagementService.Domain.Enums;

namespace WarehouseManagementService.Tests;

public sealed class ProductStatusTests
{
    [Fact]
    public void Constructor_Normalizes_Sku()
    {
        var product = new Product("Phone", " sku-1 ", 1, ProductStatus.Active);

        Assert.Equal("SKU-1", product.Sku);
    }

    [Fact]
    public void ChangeStatus_Allows_Active_To_Defective()
    {
        var product = new Product("Phone", "SKU-1", 1, ProductStatus.Active);

        product.ChangeStatus(ProductStatus.Defective);

        Assert.Equal(ProductStatus.Defective, product.Status);
    }

    [Fact]
    public void ChangeStatus_Allows_Defective_To_WriteOff()
    {
        var product = new Product("Phone", "SKU-1", 1, ProductStatus.Defective);

        product.ChangeStatus(ProductStatus.WriteOff);

        Assert.Equal(ProductStatus.WriteOff, product.Status);
    }

    [Fact]
    public void ChangeStatus_Rejects_Active_To_WriteOff()
    {
        var product = new Product("Phone", "SKU-1", 1, ProductStatus.Active);

        Assert.False(product.CanChangeStatus(ProductStatus.WriteOff));
        Assert.Throws<InvalidOperationException>(() => product.ChangeStatus(ProductStatus.WriteOff));
    }

    [Fact]
    public void ChangeStatus_Is_Idempotent_For_Same_Status()
    {
        var product = new Product("Phone", "SKU-1", 1, ProductStatus.Active);

        product.ChangeStatus(ProductStatus.Active);

        Assert.Equal(ProductStatus.Active, product.Status);
    }

    [Theory]
    [InlineData(ProductStatus.Active, ProductStatus.Defective, true)]
    [InlineData(ProductStatus.Defective, ProductStatus.WriteOff, true)]
    [InlineData(ProductStatus.WriteOff, ProductStatus.Active, false)]
    [InlineData(ProductStatus.Defective, ProductStatus.Active, false)]
    public void CanChangeStatus_Returns_Expected_Result(
        ProductStatus currentStatus,
        ProductStatus newStatus,
        bool expected)
    {
        var product = new Product("Phone", "SKU-1", 1, currentStatus);

        var result = product.CanChangeStatus(newStatus);

        Assert.Equal(expected, result);
    }
}
