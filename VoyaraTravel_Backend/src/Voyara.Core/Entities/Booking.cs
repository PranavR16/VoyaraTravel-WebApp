using System.ComponentModel.DataAnnotations;

namespace Voyara.Core.Entities;

public class Booking
{
    public Guid Id { get; set; } = Guid.NewGuid();

    [MaxLength(20)]
    public string BookingRef { get; set; } = string.Empty;

    [MaxLength(20)]
    public string Status { get; set; } = "Pending";
    // Pending | Confirmed | Cancelled | Completed

    public decimal TotalAmount { get; set; }
    public decimal TaxAmount { get; set; }
    public decimal DiscountAmount { get; set; }

    [MaxLength(20)]
    public string? CouponCode { get; set; }

    public DateTime DepartDate { get; set; }
    public DateTime ReturnDate { get; set; }

    [MaxLength(20)]
    public string FlightClass { get; set; } = "economy";

    [MaxLength(20)]
    public string RoomType { get; set; } = "std";

    [MaxLength(1000)]
    public string? SpecialRequests { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public Guid UserId { get; set; }
    public Guid PackageId { get; set; }

    public User User { get; set; } = null!;
    public Package Package { get; set; } = null!;
    public Payment? Payment { get; set; }
    public ICollection<BookingTraveler> Travelers { get; set; } = [];
    public ICollection<BookingAddon> Addons { get; set; } = [];
}