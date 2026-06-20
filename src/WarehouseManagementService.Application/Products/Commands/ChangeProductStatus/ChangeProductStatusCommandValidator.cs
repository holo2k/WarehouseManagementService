using FluentValidation;

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
            .IsInEnum()
            .When(command => command.Request is not null);
    }
}
