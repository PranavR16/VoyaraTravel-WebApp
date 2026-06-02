using System.ComponentModel.DataAnnotations;

namespace Voyara.Core.Entities;

public class Destination
{
    public Guid Id { get; set; } = Guid.NewGuid();

    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;

    [MaxLength(100)]
    public string Country { get; set; } = string.Empty;

    [MaxLength(500)]
    public string Image { get; set; } = string.Empty;

    public decimal StartingPrice { get; set; }

    [MaxLength(1000)]
    public string? Description { get; set; }

    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Navigation
    public ICollection<Package> Packages { get; set; } = [];
}
