using System;
using System.Collections.Generic;
using System.Text;

namespace Voyara.Core.DTOs.Bookings
{
        public record ValidateCouponResponseDto(
    bool IsValid,
    int DiscountPct,
    string Message
);
    }
