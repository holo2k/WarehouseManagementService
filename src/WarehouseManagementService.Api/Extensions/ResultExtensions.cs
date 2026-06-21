using Microsoft.AspNetCore.Mvc;
using WarehouseManagementService.Application.Common.Models;

namespace WarehouseManagementService.Api.Extensions;

public static class ResultExtensions
{
    public static ActionResult<ApiResponse<T>> FromResult<T>(
        this ControllerBase controller,
        Result<T> result)
    {
        if (result.IsSuccess)
        {
            return controller.Ok(ApiResponse<T>.Ok(result.Value));
        }

        return controller.FromError<T>(result.Error!);
    }

    public static ActionResult<ApiResponse<T>> FromCreatedResult<T>(
        this ControllerBase controller,
        Result<T> result,
        Func<T, string> locationFactory)
    {
        if (result.IsSuccess)
        {
            return controller.Created(
                locationFactory(result.Value),
                ApiResponse<T>.Ok(result.Value));
        }

        return controller.FromError<T>(result.Error!);
    }

    public static ActionResult<ApiResponse<T>> FromCreatedAtActionResult<T>(
        this ControllerBase controller,
        Result<T> result,
        string actionName,
        Func<T, object> routeValuesFactory)
    {
        if (result.IsSuccess)
        {
            return controller.CreatedAtAction(
                actionName,
                routeValuesFactory(result.Value),
                ApiResponse<T>.Ok(result.Value));
        }

        return controller.FromError<T>(result.Error!);
    }

    private static ObjectResult FromError<T>(this ControllerBase controller, ErrorResponse error)
    {
        var statusCode = error.Code switch
        {
            ErrorCodes.Validation => StatusCodes.Status422UnprocessableEntity,
            ErrorCodes.NotFound => StatusCodes.Status404NotFound,
            ErrorCodes.Conflict => StatusCodes.Status409Conflict,
            ErrorCodes.DatabaseConflict => StatusCodes.Status409Conflict,
            ErrorCodes.ConcurrencyConflict => StatusCodes.Status409Conflict,
            ErrorCodes.DomainRuleViolation => StatusCodes.Status422UnprocessableEntity,
            ErrorCodes.InternalServerError => StatusCodes.Status500InternalServerError,
            _ => StatusCodes.Status400BadRequest
        };

        return controller.StatusCode(statusCode, ApiResponse<T>.Fail(error));
    }
}
