namespace Voyara.Core.Entities;

public class Wishlist
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Foreign keys
    public Guid UserId { get; set; }
    public Guid PackageId { get; set; }

    // Navigation
    public User User { get; set; } = null!;
    public Package Package { get; set; } = null!;
}