using System.ComponentModel.DataAnnotations;

namespace Voyara.Core.Entities;

public class Payment
{
    public Guid Id { get; set; } = Guid.NewGuid();

    [MaxLength(100)]
    public string RazorpayOrderId { get; set; } = string.Empty;

    [MaxLength(100)]
    public string? RazorpayPaymentId { get; set; }

    [MaxLength(100)]
    public string? RazorpaySignature { get; set; }

    [MaxLength(30)]
    public string Method { get; set; } = string.Empty; // card | upi | netbanking | wallet

    [MaxLength(20)]
    public string Status { get; set; } = "Pending";    // Pending | Success | Failed

    public decimal Amount { get; set; }

    [MaxLength(5)]
    public string Currency { get; set; } = "INR";

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? PaidAt { get; set; }

    // Foreign key
    public Guid BookingId { get; set; }
    public Booking Booking { get; set; } = null!;
}