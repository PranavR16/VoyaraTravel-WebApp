using System;
using System.Collections.Generic;
using System.Text;

namespace Voyara.Core.DTOs.Payments
{
    public record VerifyPaymentDto(
    string RazorpayOrderId,
    string RazorpayPaymentId,
    string RazorpaySignature,
    Guid BookingId
);
}
