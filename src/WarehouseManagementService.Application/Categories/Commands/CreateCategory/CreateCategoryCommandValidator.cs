using FluentValidation;

namespace WarehouseManagementService.Application.Categories.Commands.CreateCategory;

public sealed class CreateCategoryCommandValidator : AbstractValidator<CreateCategoryCommand>
{
    public CreateCategoryCommandValidator()
    {
        RuleFor(command => command.Request)
            .NotNull();

        RuleFor(command => command.Request.Name)
            .NotEmpty()
            .MaximumLength(100)
            .When(command => command.Request is not null);
    }
}
