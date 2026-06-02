using System;
using System.Collections.Generic;
using System.Text;

namespace Voyara.Core.DTOs.Payments
{
    public record CreateOrderDto(
    Guid BookingId,
    string Method     // card | upi | netbanking | wallet
);
}
