using System;
using System.Collections.Generic;
using System.Text;

namespace Voyara.Core.DTOs.Shared
{
    public record ValidateCouponResponseDto(
    bool IsValid,
    int DiscountPct,
    string Message
);
}
