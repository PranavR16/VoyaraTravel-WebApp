using System;
using System.Collections.Generic;
using System.Text;

namespace Voyara.Core.DTOs.Payments
{
    public record CreateOrderResponseDto(
    string RazorpayOrderId,
    decimal Amount,
    string Currency,
    string KeyId
);
}
