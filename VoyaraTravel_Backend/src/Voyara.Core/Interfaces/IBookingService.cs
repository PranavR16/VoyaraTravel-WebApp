using Voyara.Core.DTOs.Bookings;

namespace Voyara.Core;

public interface IBookingService
{
    Task<BookingResponseDto> CreateAsync(Guid userId, CreateBookingDto dto);
    Task<IEnumerable<BookingResponseDto>> GetByUserAsync(Guid userId);
    Task<BookingResponseDto> GetByIdAsync(Guid bookingId, Guid userId);
    Task CancelAsync(Guid bookingId, Guid userId);
    Task<bool> ValidateCouponAsync(string code, decimal subtotal);
}