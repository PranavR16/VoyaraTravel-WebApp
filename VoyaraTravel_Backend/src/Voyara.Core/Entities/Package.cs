using System.ComponentModel.DataAnnotations;

namespace Voyara.Core.Entities;

public class Package
{
    public Guid Id { get; set; } = Guid.NewGuid();

    [MaxLength(200)]
    public string Name { get; set; } = string.Empty;

    [MaxLength(2000)]
    public string Description { get; set; } = string.Empty;

    public decimal Price { get; set; }
    public decimal OldPrice { get; set; }
    public int Nights { get; set; }

    [MaxLength(50)]
    public string Category { get; set; } = string.Empty;

    [MaxLength(50)]
    public string Badge { get; set; } = string.Empty;

    [MaxLength(50)]
    public string BadgeClass { get; set; } = string.Empty;

    public double Rating { get; set; }
    public int ReviewCount { get; set; }
    public int Discount { get; set; }

    [MaxLength(500)]
    public string Image { get; set; } = string.Empty;

    [MaxLength(50)]
    public string Unit { get; set; } = "per person";

    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Stored as JSON string in SQL Server
    public List<string> Tags { get; set; } = [];
    public List<string> Inclusions { get; set; } = [];

    public Guid DestinationId { get; set; }
    public Destination Destination { get; set; } = null!;
    public ICollection<Booking> Bookings { get; set; } = [];
    public ICollection<Review> ReviewList { get; set; } = [];
    public ICollection<Wishlist> Wishlists { get; set; } = [];
}
