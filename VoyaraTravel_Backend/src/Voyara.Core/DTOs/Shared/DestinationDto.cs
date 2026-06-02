using System;
using System.Collections.Generic;
using System.Text;

namespace Voyara.Core.DTOs.Shared
{
    public record DestinationDto(
    Guid Id,
    string Name,
    string Country,
    string Image,
    decimal StartingPrice,
    string? Description,
    int PackageCount
);
}
