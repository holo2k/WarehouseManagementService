using FluentValidation;

namespace WarehouseManagementService.Application.Products.Commands.CreateProduct;

public sealed class CreateProductCommandValidator : AbstractValidator<CreateProductCommand>
{
    public CreateProductCommandValidator()
    {
        RuleFor(command => command.Request)
            .NotNull();

        RuleFor(command => command.Request.Name)
            .NotEmpty()
            .MaximumLength(200)
            .When(command => command.Request is not null);

        RuleFor(command => command.Request.Sku)
            .NotEmpty()
            .MaximumLength(64)
            .When(command => command.Request is not null);

        RuleFor(command => command.Request.CategoryId)
            .GreaterThan(0)
            .When(command => command.Request is not null);

        RuleFor(command => command.Request.Status)
            .IsInEnum()
            .When(command => command.Request is not null);
    }
}
