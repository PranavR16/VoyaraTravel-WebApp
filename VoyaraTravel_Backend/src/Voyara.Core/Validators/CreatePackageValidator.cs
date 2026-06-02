using FluentValidation;
using Voyara.Core.DTOs.Packages;

namespace Voyara.Core;

public class CreatePackageValidator : AbstractValidator<CreatePackageDto>
{
    public CreatePackageValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Package name is required")
            .MaximumLength(200);

        RuleFor(x => x.Description)
            .NotEmpty().WithMessage("Description is required")
            .MaximumLength(2000);

        RuleFor(x => x.Price)
            .GreaterThan(0).WithMessage("Price must be greater than 0");

        RuleFor(x => x.OldPrice)
            .GreaterThan(0).WithMessage("Old price must be greater than 0");

        RuleFor(x => x.Nights)
            .GreaterThan(0).WithMessage("Nights must be greater than 0");

        RuleFor(x => x.Category)
            .NotEmpty().WithMessage("Category is required")
            .Must(c => new[] { "beach", "adventure", "cultural",
                               "honeymoon", "family", "luxury" }.Contains(c))
            .WithMessage("Invalid category");

        RuleFor(x => x.Image)
            .NotEmpty().WithMessage("Image URL is required");

        RuleFor(x => x.DestinationId)
            .NotEmpty().WithMessage("Destination is required");

        RuleFor(x => x.Discount)
            .InclusiveBetween(0, 100).WithMessage("Discount must be between 0 and 100");
    }
}
