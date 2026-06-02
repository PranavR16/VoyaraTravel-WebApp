using System;
using System.Collections.Generic;
using System.Text;

namespace Voyara.Core.DTOs.Bookings
{
    public record BookingAddonDto(
    string Name,
    decimal Price
);
}
