using System;
using System.Collections.Generic;
using System.Text;

namespace Voyara.Core.DTOs.Bookings
{
    public record TravelerDto(
    string Type,   // adults | children | infants
    int Count
);
}
