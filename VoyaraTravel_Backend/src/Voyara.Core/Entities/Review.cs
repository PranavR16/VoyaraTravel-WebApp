using System.ComponentModel.DataAnnotations;

namespace Voyara.Core.Entities;

public class Review
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public int Rating { get; set; }  // 1-5

    [MaxLength(1000)]
    public string Comment { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Foreign keys
    public Guid UserId { get; set; }
    public Guid PackageId { get; set; }

    // Navigation
    public User User { get; set; } = null!;
    public Package Package { get; set; } = null!;
}
