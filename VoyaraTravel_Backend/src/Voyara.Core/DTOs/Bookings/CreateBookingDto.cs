using System;
using System.Collections.Generic;
using System.Text;

namespace Voyara.Core.DTOs.Bookings
{
    public record CreateBookingDto(
    Guid PackageId,
    DateTime DepartDate,
    DateTime ReturnDate,
    string FlightClass,
    string RoomType,
    string? CouponCode,
    string? SpecialRequests,
    List<TravelerDto> Travelers,
    List<BookingAddonDto> Addons
);
}
