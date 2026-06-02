using System;
using System.Collections.Generic;
using System.Text;

namespace Voyara.Core.DTOs.Bookings
{
    public record BookingPackageDto(
    Guid Id,
    string Name,
    string Image,
    int Nights
);
}
