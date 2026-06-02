namespace Voyara.Core.Entities;

public class BookingTraveler
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Type { get; set; } = string.Empty; // adults | children | infants
    public int Count { get; set; }
    public Guid BookingId { get; set; }
    public Booking Booking { get; set; } = null!;
}