using System;
using System.Collections.Generic;
using System.Text;

namespace Voyara.Core.DTOs.Shared
{
    public record CouponDto(
    Guid Id,
    string Code,
    int DiscountPct,
    int MaxUses,
    int UsedCount,
    DateTime ExpiresAt,
    bool IsActive
);
}
