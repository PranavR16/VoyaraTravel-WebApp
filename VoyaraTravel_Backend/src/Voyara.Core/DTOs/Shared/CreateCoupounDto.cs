using System;
using System.Collections.Generic;
using System.Text;

namespace Voyara.Core.DTOs.Shared
{
    public record CreateCouponDto(
     string Code,
     int DiscountPct,
     int MaxUses,
     DateTime ExpiresAt
 );
}
