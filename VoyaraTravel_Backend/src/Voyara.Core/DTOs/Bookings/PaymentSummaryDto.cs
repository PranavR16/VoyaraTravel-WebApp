using System;
using System.Collections.Generic;
using System.Text;

namespace Voyara.Core.DTOs.Bookings
{
    public record PaymentSummaryDto(
    string Status,
    decimal Amount,
    string Method,
    DateTime? PaidAt
);
}
