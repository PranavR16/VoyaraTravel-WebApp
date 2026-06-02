using Voyara.Core;
using Voyara.Core.DTOs.Bookings;
using Voyara.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Voyara.Core.Entities;
using Voyara.Shared.Exceptions;
namespace Voyara.Infrastructure;

public class BookingService(
    VoyaraDbContext db,
    IBookingRepository bookingRepo,
    IEmailService emailService) : IBookingService
{
    public async Task<BookingResponseDto> CreateAsync(Guid userId, CreateBookingDto dto)
    {
        // Verify package exists
        var pkg = await db.Packages.FindAsync(dto.PackageId)
            ?? throw new NotFoundException("Package not found");

        // Calculate pricing
        var adults = dto.Travelers.FirstOrDefault(t => t.Type == "adults")?.Count ?? 1;
        var children = dto.Travelers.FirstOrDefault(t => t.Type == "children")?.Count ?? 0;
        var totalPax = adults + children;

        var classUpgrade = dto.FlightClass switch
        {
            "premium" => 15000m,
            "business" => 45000m,
            "first" => 90000m,
            _ => 0m
        };

        var roomUpgrade = dto.RoomType switch
        {
            "deluxe" => 8000m,
            "suite" => 25000m,
            "villa" => 60000m,
            _ => 0m
        };

        var addonsTotal = dto.Addons.Sum(a => a.Price);
        var subtotal = (pkg.Price + classUpgrade + roomUpgrade) * totalPax + addonsTotal;
        var tax = Math.Round(subtotal * 0.05m);

        // Apply coupon
        decimal discount = 0;
        if (!string.IsNullOrWhiteSpace(dto.CouponCode))
        {
            var coupon = await db.Coupons.FirstOrDefaultAsync(c =>
                c.Code == dto.CouponCode.ToUpper() &&
                c.IsActive &&
                c.ExpiresAt > DateTime.UtcNow &&
                c.UsedCount < c.MaxUses);

            if (coupon != null)
            {
                discount = Math.Round(subtotal * coupon.DiscountPct / 100m);
                coupon.UsedCount++;
            }
        }

        var total = subtotal + tax - discount;

        // Generate booking reference
        var bookingRef = $"VOY-{DateTime.UtcNow:yyyy}-{Guid.NewGuid().ToString()[..6].ToUpper()}";

        var booking = new Booking
        {
            BookingRef = bookingRef,
            Status = "Pending",
            TotalAmount = total,
            TaxAmount = tax,
            DiscountAmount = discount,
            CouponCode = dto.CouponCode?.ToUpper(),
            DepartDate = dto.DepartDate,
            ReturnDate = dto.ReturnDate,
            FlightClass = dto.FlightClass,
            RoomType = dto.RoomType,
            SpecialRequests = dto.SpecialRequests,
            UserId = userId,
            PackageId = dto.PackageId,
            Travelers = dto.Travelers.Select(t => new BookingTraveler
            {
                Type = t.Type,
                Count = t.Count
            }).ToList(),
            Addons = dto.Addons.Select(a => new BookingAddon
            {
                Name = a.Name,
                Price = a.Price
            }).ToList()
        };

        await bookingRepo.AddAsync(booking);

        // Send confirmation email (fire and forget)
        var user = await db.Users.FindAsync(userId);
        if (user != null)
            _ = emailService.SendBookingConfirmationAsync(
                user.Email, user.Name, bookingRef, pkg.Name, total);

        return await MapToDto(booking.Id);
    }

    public async Task<IEnumerable<BookingResponseDto>> GetByUserAsync(Guid userId)
    {
        var bookings = await bookingRepo.GetByUserIdAsync(userId);
        return bookings.Select(MapToResponseDto);
    }

    public async Task<BookingResponseDto> GetByIdAsync(Guid bookingId, Guid userId)
    {
        var booking = await bookingRepo.GetByIdWithDetailsAsync(bookingId)
            ?? throw new NotFoundException("Booking not found");

        if (booking.UserId != userId)
            throw new UnauthorizedException("Access denied");

        return MapToResponseDto(booking);
    }

    public async Task CancelAsync(Guid bookingId, Guid userId)
    {
        var booking = await bookingRepo.GetByIdAsync(bookingId)
            ?? throw new NotFoundException("Booking not found");

        if (booking.UserId != userId)
            throw new UnauthorizedException("Access denied");

        if (booking.Status == "Cancelled")
            throw new BadRequestException("Booking is already cancelled");

        if (booking.Status == "Completed")
            throw new BadRequestException("Cannot cancel a completed booking");

        booking.Status = "Cancelled";
        await bookingRepo.UpdateAsync(booking);
    }

    public async Task<bool> ValidateCouponAsync(string code, decimal subtotal)
    {
        var coupon = await db.Coupons.FirstOrDefaultAsync(c =>
            c.Code == code.ToUpper() &&
            c.IsActive &&
            c.ExpiresAt > DateTime.UtcNow &&
            c.UsedCount < c.MaxUses);

        return coupon != null;
    }

    // ── Helpers ──────────────────────────────────────────
    private async Task<BookingResponseDto> MapToDto(Guid bookingId)
    {
        var b = await bookingRepo.GetByIdWithDetailsAsync(bookingId)
            ?? throw new NotFoundException("Booking not found");
        return MapToResponseDto(b);
    }

    private static BookingResponseDto MapToResponseDto(Booking b) => new(
        b.Id,
        b.BookingRef,
        b.Status,
        b.TotalAmount,
        b.TaxAmount,
        b.DiscountAmount,
        b.CouponCode,
        b.DepartDate,
        b.ReturnDate,
        b.FlightClass,
        b.RoomType,
        b.SpecialRequests,
        b.CreatedAt,
        new BookingPackageDto(
            b.Package.Id,
            b.Package.Name,
            b.Package.Image,
            b.Package.Nights),
        b.Travelers.Select(t => new TravelerDto(t.Type, t.Count)).ToList(),
        b.Addons.Select(a => new BookingAddonDto(a.Name, a.Price)).ToList(),
        b.Payment == null ? null : new PaymentSummaryDto(
            b.Payment.Status,
            b.Payment.Amount,
            b.Payment.Method,
            b.Payment.PaidAt)
    );
}
 
