using System;
using System.Collections.Generic;
using System.Text;

namespace Voyara.Core.DTOs.Payments
{
    public record PaymentResponseDto(
    bool Success,
    string BookingRef,
    string Message
);
}
