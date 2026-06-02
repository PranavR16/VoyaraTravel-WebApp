using System;
using System.Collections.Generic;
using System.Text;

namespace Voyara.Core.DTOs.Bookings
{
    public record BookingResponseDto(
    Guid Id,
    string BookingRef,
    string Status,
    decimal TotalAmount,
    decimal TaxAmount,
    decimal DiscountAmount,
    string? CouponCode,
    DateTime DepartDate,
    DateTime ReturnDate,
    string FlightClass,
    string RoomType,
    string? SpecialRequests,
    DateTime CreatedAt,
    BookingPackageDto Package,
    List<TravelerDto> Travelers,
    List<BookingAddonDto> Addons,
    PaymentSummaryDto? Payment
);
}
