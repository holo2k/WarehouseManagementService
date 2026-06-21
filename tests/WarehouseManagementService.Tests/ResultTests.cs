using WarehouseManagementService.Application.Common.Models;

namespace WarehouseManagementService.Tests;

public sealed class ResultTests
{
    [Fact]
    public void Success_Returns_Value()
    {
        var result = Result.Success("created");

        Assert.True(result.IsSuccess);
        Assert.False(result.IsFailure);
        Assert.Equal("created", result.Value);
        Assert.Null(result.Error);
    }

    [Fact]
    public void Failure_Returns_Error()
    {
        var result = Result.Failure<string>(ErrorCodes.NotFound, "Not found.");

        Assert.False(result.IsSuccess);
        Assert.True(result.IsFailure);
        Assert.Equal(ErrorCodes.NotFound, result.Error?.Code);
        Assert.Equal("Not found.", result.Error?.Message);
    }

    [Fact]
    public void Failure_Created_By_Value_Type_Returns_Failed_Result()
    {
        var error = new ErrorResponse(ErrorCodes.Validation, "Validation failed.");

        var result = (Result<string>)Result.Failure(typeof(string), error);

        Assert.True(result.IsFailure);
        Assert.Equal(error, result.Error);
    }
}
