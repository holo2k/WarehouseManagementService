using FluentValidation;
using WarehouseManagementService.Application.Products;

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
            .OverridePropertyName(nameof(CreateProductRequest.Name))
            .When(command => command.Request is not null);

        RuleFor(command => command.Request.Sku)
            .NotEmpty()
            .MaximumLength(64)
            .OverridePropertyName(nameof(CreateProductRequest.Sku))
            .When(command => command.Request is not null);

        RuleFor(command => command.Request.CategoryId)
            .GreaterThan(0)
            .OverridePropertyName(nameof(CreateProductRequest.CategoryId))
            .When(command => command.Request is not null);

        RuleFor(command => command.Request.Status)
            .Must(status => status is null || ProductStatusParser.TryParse(status, out _))
            .WithMessage($"Status must be one of: {ProductStatusParser.AllowedValues}.")
            .OverridePropertyName(nameof(CreateProductRequest.Status))
            .When(command => command.Request is not null);
    }
}
