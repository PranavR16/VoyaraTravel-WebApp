using System.ComponentModel.DataAnnotations;

namespace Voyara.Core.Entities;

public class Coupon
{
    public Guid Id { get; set; } = Guid.NewGuid();

    [MaxLength(20)]
    public string Code { get; set; } = string.Empty;

    public int DiscountPct { get; set; }
    public int MaxUses { get; set; }
    public int UsedCount { get; set; } = 0;
    public DateTime ExpiresAt { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
