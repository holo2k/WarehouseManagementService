using FluentValidation;
using WarehouseManagementService.Application.Products;

namespace WarehouseManagementService.Application.Products.Commands.ChangeProductStatus;

public sealed class ChangeProductStatusCommandValidator : AbstractValidator<ChangeProductStatusCommand>
{
    public ChangeProductStatusCommandValidator()
    {
        RuleFor(command => command.Id)
            .GreaterThan(0);

        RuleFor(command => command.Request)
            .NotNull();

        RuleFor(command => command.Request.Status)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .Must(status => ProductStatusParser.TryParse(status, out _))
            .WithMessage($"Status must be one of: {ProductStatusParser.AllowedValues}.")
            .OverridePropertyName(nameof(ChangeProductStatusRequest.Status))
            .When(command => command.Request is not null);
    }
}
