using FluentValidation;
using Voyara.Core.DTOs.Bookings;

namespace Voyara.Core;

public class CreateBookingValidator : AbstractValidator<CreateBookingDto>
{
    public CreateBookingValidator()
    {
        RuleFor(x => x.PackageId)
            .NotEmpty().WithMessage("Package is required");

        RuleFor(x => x.DepartDate)
            .NotEmpty().WithMessage("Departure date is required")
            .GreaterThan(DateTime.UtcNow).WithMessage("Departure date must be in the future");

        RuleFor(x => x.ReturnDate)
            .NotEmpty().WithMessage("Return date is required")
            .GreaterThan(x => x.DepartDate).WithMessage("Return date must be after departure date");

        RuleFor(x => x.FlightClass)
            .NotEmpty().WithMessage("Flight class is required")
            .Must(c => new[] { "economy", "premium", "business", "first" }.Contains(c))
            .WithMessage("Invalid flight class");

        RuleFor(x => x.RoomType)
            .NotEmpty().WithMessage("Room type is required")
            .Must(r => new[] { "std", "deluxe", "suite", "villa" }.Contains(r))
            .WithMessage("Invalid room type");

        RuleFor(x => x.Travelers)
            .NotEmpty().WithMessage("At least one traveler is required");

        RuleForEach(x => x.Travelers).ChildRules(t =>
        {
            t.RuleFor(x => x.Type)
                .Must(type => new[] { "adults", "children", "infants" }.Contains(type))
                .WithMessage("Invalid traveler type");

            t.RuleFor(x => x.Count)
                .GreaterThan(0).WithMessage("Traveler count must be greater than 0");
        });

        RuleFor(x => x.Travelers)
            .Must(travelers =>
            {
                var adults = travelers.FirstOrDefault(t => t.Type == "adults");
                return adults != null && adults.Count >= 1;
            })
            .WithMessage("At least 1 adult is required");
    }
}
